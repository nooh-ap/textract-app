using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace TextractApi.Models;

[DynamoDBTable("textract")]
public class DynamoDbItemDto    
{
    [DynamoDBHashKey]
    public string id { get; set; }
    
    [DynamoDBProperty("item")]
    public string Item { get; set; }
    
    [DynamoDBProperty]
    public decimal price { get; set; }
    
    [DynamoDBProperty]
    public int quantity { get; set; }
}