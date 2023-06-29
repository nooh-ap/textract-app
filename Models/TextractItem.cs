using System;
namespace TextractApi.Models
{
    public class TextractItem
	{
        public Guid Id { get; set; }
        public required string Item { get; set; }
        public required decimal Amount { get; set; }
        public string? Secret { get; set; }
    }

    public class SummaryFields
    {
        public string Type { get; set; }
        public string Value { get; set; }
        //public string? Secret { get; set; }
    }    


    public class LineItems
    {
        public string Type { get; set; }
        public string Value { get; set; }

    }

    public class ExpenseItem
    {
        public Guid Id { get; set; }
        public string Item { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}

