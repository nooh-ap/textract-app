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
}

