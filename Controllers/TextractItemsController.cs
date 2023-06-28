using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextractApi.Models;

namespace TextractApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextractItemsController : ControllerBase
    {
        private readonly TextractContext _context;

        public TextractItemsController(TextractContext context)
        {
            _context = context;
        }

        // GET: api/TextractItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TextractItemDTO>>> GetTextractItems()
        {
            return await _context.TextractItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // GET: api/TextractItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TextractItemDTO>> GetTextractItem(Guid id)
        {
            var TextractItem = await _context.TextractItems.FindAsync(id);

            if (TextractItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(TextractItem);
        }

        // PUT: api/TextractItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTextractItem(Guid id, TextractItemDTO textractDTO)
        {
            if (id != textractDTO.Id)
            {
                return BadRequest();
            }

            var todoItem = await _context.TextractItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Id = textractDTO.Id;
            todoItem.Amount= textractDTO.Amount;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TextractItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TextractItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TextractItemDTO>> PostTextractItem(TextractItemDTO textractDTO)
        {
            var textractItem = new TextractItem
            {
                Item = textractDTO.Item,
                Amount = textractDTO.Amount
            };

            _context.TextractItems.Add(textractItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTextractItem),
                new { id = textractItem.Id },
                ItemToDTO(textractItem));
        }

        // DELETE: api/TextractItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTextractItem(Guid id)
        {
            if (_context.TextractItems == null)
            {
                return NotFound();
            }
            var textractItem = await _context.TextractItems.FindAsync(id);
            if (textractItem == null)
            {
                return NotFound();
            }

            _context.TextractItems.Remove(textractItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TextractItemExists(Guid id)
        {
            return (_context.TextractItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static TextractItemDTO ItemToDTO(TextractItem textractItem) =>
            new TextractItemDTO
            {
                Id = textractItem.Id,
                Item = textractItem.Item,
                Amount = textractItem.Amount
            };
     
    }
}
