using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using TextractApi.Models;
using S3Object = Amazon.Textract.Model.S3Object;

namespace TextractApi.Controllers.TextractControllers;

[Route("api/[controller]")]
[ApiController]
public class TextractController : ControllerBase
{
    private readonly IDynamoDBContext _dynamoDbContext;

    public TextractController(
        IDynamoDBContext dynamoDbContext
    )
    {
        _dynamoDbContext = dynamoDbContext;
    }
    private const string BucketName = "textractapi-bucketa";

    // GET: api/Textract
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET: api/Textract/5
    [HttpGet("{id}", Name = "Get")]
    public string Get(int id)
    {
        return "value";
    }

    // POST: api/Textract
    [HttpPost("analyzeReciept")]
    public async Task<IActionResult> AnalyzeReciept(IFormFile file)
    {
        return await AnalyzeExpense(file);
    }


    private async Task<DocumentLocation> UploadFile(IFormFile formFile)
    {
        var client = new AmazonS3Client();
        var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, BucketName);
        if (!bucketExists)
        {
            // Create a new bucket
            var bucketRequest = new PutBucketRequest
                                {
                                    BucketName = BucketName,
                                    UseClientRegion = true
                                };
            await client.PutBucketAsync(bucketRequest);
        }

        // Upload File to S3
        var objectRequest = new PutObjectRequest
                            {
                                BucketName = BucketName,
                                Key = $"{DateTime.Now:yyyyy\\/MM\\/dd\\/hmmss}--{formFile.FileName}",
                                InputStream = formFile.OpenReadStream()
                            };

        await client.PutObjectAsync(objectRequest);


        return new DocumentLocation
               {
                   S3Object = new S3Object
                              {
                                  Bucket = objectRequest.BucketName,
                                  Name = objectRequest.Key
                              }
               };
    }

    private async Task<IActionResult> AnalyzeExpense(IFormFile file)
    {
        var documentLocation = await UploadFile(file);
        // Setup Job 
        var startJobRequest = new StartExpenseAnalysisRequest { DocumentLocation = documentLocation };
        var textractClient = new AmazonTextractClient();
        var startJobResponse = await textractClient.StartExpenseAnalysisAsync(startJobRequest);
        var getResultsRequest = new GetExpenseAnalysisRequest { JobId = startJobResponse.JobId };

        // Retrieve initial response
        var getResultsResponse =
            await textractClient.GetExpenseAnalysisAsync(getResultsRequest);

        // Check Job Status 
        while (getResultsResponse.JobStatus == JobStatus.IN_PROGRESS)
        {
            Thread.Sleep(1000);
            // Update Response Status
            getResultsResponse = await textractClient.GetExpenseAnalysisAsync(getResultsRequest);
        }

        // Process Data in Structure
        if (getResultsResponse.JobStatus == JobStatus.SUCCEEDED)
        {
            var summary = getResultsResponse.ExpenseDocuments[0].SummaryFields;
            var items = getResultsResponse.ExpenseDocuments[0].LineItemGroups[0].LineItems;


            // Extract Summary Fields
            var summaryFields =
                (from block in summary
                 where block.Type.Text != "OTHER"
                 select new SummaryFields
                        {
                            Type = block.Type.Text,
                            Value = block.ValueDetection.Text
                        }
                ).ToList();

            // create object keys for each item
            var summaryKeys =
                summaryFields.Select(x => x.Type).ToList();
            var summaryValues =
                summaryFields.Select(x => x.Value).ToList();

            var summaryDictionary = new Dictionary<string, string>();

            for (var i = 0; i < summaryKeys.Count; i++)
                if (summaryDictionary.ContainsKey(summaryKeys[i]) == false)
                    summaryDictionary.Add(summaryKeys[i], summaryValues[i]);


            // Exract Line Items
            var expenseItems =
                (from item in items
                 from expense in item.LineItemExpenseFields
                 select new LineItems
                        {
                            Type = expense.Type.Text,
                            Value = expense.ValueDetection.Text
                        }).ToList();


            var expenseItemsKeys = expenseItems.Select(x => x.Type).ToList();
            var expenseItemsValues = expenseItems.Select(x => x.Value).ToList();

            var expenseItemsDictionary = new Dictionary<string, string>();
            
            for (var i = 0; i < expenseItemsKeys.Count; i++)
                if (expenseItemsDictionary.ContainsKey(expenseItemsKeys[i]) == false)
                    expenseItemsDictionary.Add(expenseItemsKeys[i], expenseItemsValues[i]);

            var expenseItemsList = expenseItemsDictionary.ToList();
            
            
            // Serialize to JSON
            var summaryJson = JsonConvert.SerializeObject(summaryDictionary);
            
            // Store in one object
            var recieptAnalysis = new RecieptAnalysis
                                  {
                                      id = Guid.NewGuid().ToString(),
                                      summary = JsonConvert.SerializeObject(summaryDictionary),
                                      items = JsonConvert.SerializeObject(expenseItemsList)
                                  };


            // Save to DynamoDB - Textract Table    
            await _dynamoDbContext.SaveAsync(recieptAnalysis);
            
            return Ok(recieptAnalysis);
        }


        return Ok();
    }
}

[DynamoDBTable("textract")]
internal class RecieptAnalysis
{
    [DynamoDBHashKey]
    public string id { get; set; }
    
    [DynamoDBProperty("summary")]
    public string summary { get; set; }
    
    [DynamoDBProperty("items")]
    public string items { get; set; }
}