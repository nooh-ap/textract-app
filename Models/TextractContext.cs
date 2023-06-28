
using System;
using Microsoft.EntityFrameworkCore;


namespace TextractApi.Models
{
	public class TextractContext : DbContext
	{
		public TextractContext(DbContextOptions<TextractContext> options)
			:base(options)
		{
		}

		public DbSet<TextractItem> TextractItems { get; set; } = null!;

	}
}

