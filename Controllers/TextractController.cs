using Microsoft.AspNetCore.Mvc;

namespace TextractApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TextractController : ControllerBase
{
    private static readonly string[] Items = new[]
    {
        "Spinach",
        "Berries",
        "Corn",
        "Watermelon",
        "Water",
        "Cheese",
        "Hot Dog",
        "Sauce",
        "Salad",
        "Flower"
    };

    private readonly ILogger<TextractController> _logger;

    public TextractController(ILogger<TextractController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetTextract")]
    public IEnumerable<Textract> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new Textract
        {
            Id = Guid.NewGuid(),
            Item = Items[Random.Shared.Next(Items.Length)], 
            Amount = Random.Shared.Next(1, 55)
        })
        .ToArray();
    }
}

