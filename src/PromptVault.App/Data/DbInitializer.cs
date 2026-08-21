using Microsoft.EntityFrameworkCore;
using PromptVault.App.Models;

namespace PromptVault.App.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(Func<AppDbContext> dbFactory)
    {
        await using var db = dbFactory();
        await db.Database.EnsureCreatedAsync();

        if (await db.Categories.AnyAsync())
        {
            return;
        }

        var categories = new[]
        {
            new Category { Name = "General" },
            new Category { Name = "Desarrollo" },
            new Category { Name = "Investigación" },
            new Category { Name = "Marketing" },
            new Category { Name = "Imágenes" },
            new Category { Name = "Documentación" },
            new Category { Name = "Juegos y narrativa" }
        };
        db.Categories.AddRange(categories);

        var developmentTag = new Tag { Name = "desarrollo" };
        var architectureTag = new Tag { Name = "arquitectura" };
        db.Tags.AddRange(developmentTag, architectureTag);

        var now = DateTime.UtcNow;
        var samplePrompt = new PromptItem
        {
            Title = "Arquitecto de software",
            Description = "Analiza una aplicación existente antes de implementar una función.",
            Content = "Actúa como product engineer senior y arquitecto de software.\n\nAnaliza el proyecto {{nombre_proyecto}} y propone una implementación integrada para: {{objetivo}}.\n\nReutiliza patrones existentes, evita duplicar lógica y valida la solución antes de terminar.",
            Category = categories[1],
            Model = "General",
            IsFavorite = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        samplePrompt.PromptTags.Add(new PromptTag { PromptItem = samplePrompt, Tag = developmentTag });
        samplePrompt.PromptTags.Add(new PromptTag { PromptItem = samplePrompt, Tag = architectureTag });
        db.Prompts.Add(samplePrompt);

        await db.SaveChangesAsync();
    }
}
