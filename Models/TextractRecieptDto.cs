namespace TextractApi.Models;

public class TextractRecieptDto
{
    public class ResponseItem
    {
        public string currency { get; set; }
        public List<GroupProperty> groupProperties { get; set; }
        public object labelDetection { get; set; }
        public int pageNumber { get; set; }
        public TypeData type { get; set; }
        public ValueData valueDetection { get; set; }
    }

    public class GroupProperty
    {
        public string id { get; set; }
        public List<string> types { get; set; }
    }

    public class TypeData
    {
        public double confidence { get; set; }
        public string text { get; set; }
    }

    public class ValueData
    {
        public double confidence { get; set; }
        public Geometry geometry { get; set; }
        public string text { get; set; }
    }

    public class Geometry
    {
        public BoundingBox boundingBox { get; set; }
        public List<Point> polygon { get; set; }
    }

    public class BoundingBox
    {
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double width { get; set; }
    }

    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }
    }

}