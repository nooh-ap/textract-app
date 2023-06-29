using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using TextractApi.Models;

namespace TextractApi.Controllers.DynamoDBControllers;

[Route("api/[controller]")]
[ApiController]
public class DynamoDbController : ControllerBase
{
    private readonly IDynamoDBContext _dynamoDbContext;

    public DynamoDbController(
        IDynamoDBContext dynamoDbContext
    )
    {
        _dynamoDbContext = dynamoDbContext;
    }

    // GET: api/API
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET: api/API/5
    [HttpGet("getItem")]
    public async Task<DynamoDbItemDto> Get(string id)
    {
        return await _dynamoDbContext.LoadAsync<DynamoDbItemDto>(id);
    }

    // POST: api/API
    [HttpPost]
    public async Task Post(DynamoDbItemDto value)
    {
        await _dynamoDbContext.SaveAsync(value);
    }

    // PUT: api/API/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/API/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}