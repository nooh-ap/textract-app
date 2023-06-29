using System.Text.Json.Serialization;

namespace TextractApi.Models;

public class TextracractExpenseDto
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);
    public class BoundingBox
    {
        [JsonPropertyName("height")]
        public double height { get; set; }

        [JsonPropertyName("left")]
        public double left { get; set; }

        [JsonPropertyName("top")]
        public double top { get; set; }

        [JsonPropertyName("width")]
        public double width { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("boundingBox")]
        public BoundingBox boundingBox { get; set; }

        [JsonPropertyName("polygon")]
        public List<Polygon> polygon { get; set; }
    }

    public class LineItemExpenseField
    {
        [JsonPropertyName("currency")]
        public object currency { get; set; }

        [JsonPropertyName("groupProperties")]
        public List<object> groupProperties { get; set; }

        [JsonPropertyName("labelDetection")]
        public object labelDetection { get; set; }

        [JsonPropertyName("pageNumber")]
        public int pageNumber { get; set; }

        [JsonPropertyName("type")]
        public Type type { get; set; }

        [JsonPropertyName("valueDetection")]
        public ValueDetection valueDetection { get; set; }
    }

    public class Polygon
    {
        [JsonPropertyName("x")]
        public double x { get; set; }

        [JsonPropertyName("y")]
        public double y { get; set; }
    }

    public class Root
    {
        [JsonPropertyName("lineItemExpenseFields")]
        public List<LineItemExpenseField> lineItemExpenseFields { get; set; }
    }

    public class Type
    {
        [JsonPropertyName("confidence")]
        public double confidence { get; set; }

        [JsonPropertyName("text")]
        public string text { get; set; }
    }

    public class ValueDetection
    {
        [JsonPropertyName("confidence")]
        public double confidence { get; set; }

        [JsonPropertyName("geometry")]
        public Geometry geometry { get; set; }

        [JsonPropertyName("text")]
        public string text { get; set; }
    }


}