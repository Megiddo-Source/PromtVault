using Microsoft.EntityFrameworkCore;
using PromptVault.App.Models;

namespace PromptVault.App.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PromptItem> Prompts => Set<PromptItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PromptTag> PromptTags => Set<PromptTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PromptItem>(entity =>
        {
            entity.Property(item => item.Title).IsRequired().HasMaxLength(180);
            entity.Property(item => item.Description).HasMaxLength(500);
            entity.Property(item => item.Content).IsRequired();
            entity.Property(item => item.Model).HasMaxLength(80);
            entity.HasOne(item => item.Category)
                .WithMany(category => category.Prompts)
                .HasForeignKey(item => item.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(item => item.Name)
                .IsRequired()
                .HasMaxLength(100)
                .UseCollation("NOCASE");
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(item => item.Name)
                .IsRequired()
                .HasMaxLength(80)
                .UseCollation("NOCASE");
            entity.HasIndex(item => item.Name).IsUnique();
        });

        modelBuilder.Entity<PromptTag>(entity =>
        {
            entity.HasKey(item => new { item.PromptItemId, item.TagId });
            entity.HasOne(item => item.PromptItem)
                .WithMany(prompt => prompt.PromptTags)
                .HasForeignKey(item => item.PromptItemId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.Tag)
                .WithMany(tag => tag.PromptTags)
                .HasForeignKey(item => item.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
