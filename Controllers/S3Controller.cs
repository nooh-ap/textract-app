using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Mvc;
using TextractApi.Models;

namespace TextractApi.Controllers
{
    [Route("[controller]")]
    public class S3Controller : Controller
    {

        private const string BucketName = "textractapi-bucketa";

        // GET: api/values
        [HttpGet("getFiles")]
        public async Task<IActionResult> GetFiles(string prefix)
        {
            var client = new AmazonS3Client();
            var request = new ListObjectsV2Request()
            {
                BucketName = BucketName,
                Prefix = prefix
            };
            var response = await client.ListObjectsV2Async(request);
            var presignedUrls = response.S3Objects.Select(o =>
            {
                var req = new GetPreSignedUrlRequest()
                {
                    BucketName = BucketName,
                    Key = o.Key,
                    Expires = DateTime.UtcNow.AddSeconds(30)
                };
            return client.GetPreSignedURL(req);
            });
            return Ok(presignedUrls);
        }

        // GET Files that are in S3
        [HttpGet("getFile")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            var client = new AmazonS3Client();
            var response = await client.GetObjectAsync(BucketName, fileName);

            using var reader = new StreamReader(response.ResponseStream);
            // var fileContents = await reader.ReadToEndAsync();  

            return File(response.ResponseStream, response.Headers.ContentType);
        }


        // POST api/values
        // [HttpPost("uploadFile")]
        // public async Task<ActionResult> AnalyzeExpense(IFormFile file)
        // {
        //
        //     var documentLocation = await UploadFile(file);
        //     // Setup Job 
        //     var startJobRequest = new StartExpenseAnalysisRequest { DocumentLocation = documentLocation };
        //     var textractClient = new AmazonTextractClient();
        //     var startJobResponse = await textractClient.StartExpenseAnalysisAsync(startJobRequest);
        //     var getResultsRequest = new GetExpenseAnalysisRequest { JobId = startJobResponse.JobId };
        //
        //     // Retrieve initial response
        //     var getResultsResponse =
        //         await textractClient.GetExpenseAnalysisAsync(getResultsRequest);
        //
        //     // Check Job Status 
        //     while (getResultsResponse.JobStatus == JobStatus.IN_PROGRESS)
        //     {
        //         Thread.Sleep(1000);
        //         // Update Response Status
        //         getResultsResponse = await textractClient.GetExpenseAnalysisAsync(getResultsRequest);
        //     }
        //
        //     // Process Data in Structure
        //     if (getResultsResponse.JobStatus == JobStatus.SUCCEEDED)
        //     {
        //         var summary = getResultsResponse.ExpenseDocuments[0].SummaryFields;
        //         var items = getResultsResponse.ExpenseDocuments[0].LineItemGroups[0].LineItems;
        //
        //         // Store data in Object
        //         var summaryFields =
        //             (from block in summary
        //              where block.Type.Text != "OTHER"
        //              select new SummaryFields
        //                     {
        //                         Id = Guid.NewGuid(),
        //                         Type = block.Type.Text,
        //                         Text = block.ValueDetection.Text
        //                     }
        //             ).ToList();
        //
        //         var expenseItems =
        //             (from item in items
        //              from expense in item.LineItemExpenseFields
        //              select new LineItems
        //                     {
        //                         Id = Guid.NewGuid(),
        //                         Item = expense.Type.Text,
        //                         Value = expense.ValueDetection.Text
        //                     }).ToList();
        //
        //         return Ok(expenseItems);
        //     }
        //
        //
        //     return Ok();
        //         
        // }

        // TODO: Add Multi files Upload
        // TODO: Move Textract Actions into a separate folder   
        // TODO: Refactor the Analyze Job setup    
        

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        
        // Upload File to S3
        private async Task<DocumentLocation> UploadFile(IFormFile formFile)
        {
            var client = new AmazonS3Client();
            var bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, BucketName);
            if (!bucketExists)
            {
                // Create a new bucket
                var bucketRequest = new PutBucketRequest()
                                    {
                                        BucketName = BucketName,
                                        UseClientRegion = true
                                    };
                await client.PutBucketAsync(bucketRequest);
            }

            // Upload File to S3
            var objectRequest = new PutObjectRequest()
                                {
                                    BucketName = BucketName,
                                    Key = $"{DateTime.Now:yyyyy\\/MM\\/dd\\/hmmss}--{formFile.FileName}",
                                    InputStream = formFile.OpenReadStream(),
                                };

            await client.PutObjectAsync(objectRequest);


            return new DocumentLocation
                   {
                       S3Object = new Amazon.Textract.Model.S3Object
                                  {
                                      Bucket = objectRequest.BucketName,
                                      Name = objectRequest.Key
                                  }
                   };
        }
        }
}

