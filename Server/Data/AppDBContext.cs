using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Data;

public class AppDBContext : DbContext
{
    public DbSet<Post> Categories { get; set; }

	public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call the base version of this method as well, else we get an error later on.
        base.OnModelCreating(modelBuilder);

        Post[] categoriesToSeed = new Post[3];

        for (int i = 1; i < 4; i++)
        {
            categoriesToSeed[i - 1] = new Post
            {
                CategoryId = i,
                ThumbnailImagePath = "uploads/placeholder.jpg",
                Name = $"Category {i}",
                Description = $"A description of category {i}"
            };
        }

        modelBuilder.Entity<Post>().HasData(categoriesToSeed);
    }
}
