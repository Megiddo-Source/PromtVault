using Microsoft.EntityFrameworkCore;
using PromptVault.App.Data;
using PromptVault.App.Models;

namespace PromptVault.App.Services;

public sealed class PromptService : IPromptService
{
    private readonly Func<AppDbContext> _dbFactory;

    public PromptService(Func<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<IReadOnlyList<PromptItem>> GetPromptsAsync(string? searchText, PromptFilter filter)
    {
        await using var db = _dbFactory();
        var prompts = await BaseQuery(db)
            .Where(item => !item.IsArchived)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            prompts = prompts.Where(item => Matches(item, term)).ToList();
        }

        return filter switch
        {
            PromptFilter.Favorites => prompts
                .Where(item => item.IsFavorite)
                .OrderByDescending(item => item.UpdatedAt)
                .ToList(),
            PromptFilter.MostUsed => prompts
                .OrderByDescending(item => item.UseCount)
                .ThenBy(item => item.Title)
                .ToList(),
            PromptFilter.Recent => prompts
                .OrderByDescending(item => item.LastUsedAt ?? item.UpdatedAt)
                .ToList(),
            _ => prompts
                .OrderByDescending(item => item.IsFavorite)
                .ThenByDescending(item => item.UpdatedAt)
                .ToList()
        };
    }

    public async Task<IReadOnlyList<PromptItem>> GetAllForExportAsync()
    {
        await using var db = _dbFactory();
        return await BaseQuery(db)
            .OrderBy(item => item.Title)
            .ToListAsync();
    }

    public async Task<PromptItem?> GetByIdAsync(int id)
    {
        await using var db = _dbFactory();
        return await BaseQuery(db).FirstOrDefaultAsync(item => item.Id == id);
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        await using var db = _dbFactory();
        return await db.Categories.AsNoTracking().OrderBy(item => item.Name).ToListAsync();
    }

    public async Task<int> SaveAsync(PromptEditData data)
    {
        if (string.IsNullOrWhiteSpace(data.Title))
        {
            throw new ArgumentException("El título es obligatorio.", nameof(data));
        }

        if (string.IsNullOrWhiteSpace(data.Content))
        {
            throw new ArgumentException("El contenido es obligatorio.", nameof(data));
        }

        await using var db = _dbFactory();
        var now = DateTime.UtcNow;
        PromptItem entity;

        if (data.Id.HasValue)
        {
            entity = await db.Prompts
                .Include(item => item.PromptTags)
                .SingleAsync(item => item.Id == data.Id.Value);
            db.PromptTags.RemoveRange(entity.PromptTags);
            entity.PromptTags.Clear();
        }
        else
        {
            entity = new PromptItem
            {
                CreatedAt = data.CreatedAt ?? now,
                UseCount = data.UseCount ?? 0,
                LastUsedAt = data.LastUsedAt
            };
            db.Prompts.Add(entity);
        }

        entity.Title = data.Title.Trim();
        entity.Description = data.Description.Trim();
        entity.Content = data.Content.Trim();
        entity.Model = string.IsNullOrWhiteSpace(data.Model) ? "General" : data.Model.Trim();
        entity.IsFavorite = data.IsFavorite;
        entity.UpdatedAt = data.UpdatedAt ?? now;

        if (data.UseCount.HasValue)
        {
            entity.UseCount = data.UseCount.Value;
            entity.LastUsedAt = data.LastUsedAt;
        }

        if (string.IsNullOrWhiteSpace(data.CategoryName))
        {
            entity.Category = null;
            entity.CategoryId = null;
        }
        else
        {
            var categoryName = data.CategoryName.Trim();
            var category = await db.Categories.FirstOrDefaultAsync(item => item.Name == categoryName);
            if (category is null)
            {
                category = new Category { Name = categoryName };
                db.Categories.Add(category);
            }
            entity.Category = category;
        }

        foreach (var tagName in ParseTags(data.TagsText))
        {
            var tag = await db.Tags.FirstOrDefaultAsync(item => item.Name == tagName);
            if (tag is null)
            {
                tag = new Tag { Name = tagName };
                db.Tags.Add(tag);
            }
            entity.PromptTags.Add(new PromptTag { PromptItem = entity, Tag = tag });
        }

        await db.SaveChangesAsync();
        return entity.Id;
    }

    public async Task DeleteAsync(int id)
    {
        await using var db = _dbFactory();
        var prompt = await db.Prompts.FindAsync(id);
        if (prompt is null)
        {
            return;
        }

        db.Prompts.Remove(prompt);
        await db.SaveChangesAsync();
    }

    public async Task ToggleFavoriteAsync(int id)
    {
        await using var db = _dbFactory();
        var prompt = await db.Prompts.FindAsync(id);
        if (prompt is null)
        {
            return;
        }

        prompt.IsFavorite = !prompt.IsFavorite;
        prompt.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task RecordUseAsync(int id)
    {
        await using var db = _dbFactory();
        var prompt = await db.Prompts.FindAsync(id);
        if (prompt is null)
        {
            return;
        }

        prompt.UseCount += 1;
        prompt.LastUsedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    private static IQueryable<PromptItem> BaseQuery(AppDbContext db) => db.Prompts
        .AsNoTracking()
        .Include(item => item.Category)
        .Include(item => item.PromptTags)
        .ThenInclude(item => item.Tag);

    private static bool Matches(PromptItem prompt, string term)
    {
        var comparison = StringComparison.CurrentCultureIgnoreCase;
        return prompt.Title.Contains(term, comparison)
            || prompt.Description.Contains(term, comparison)
            || prompt.Content.Contains(term, comparison)
            || prompt.Model.Contains(term, comparison)
            || (prompt.Category?.Name.Contains(term, comparison) ?? false)
            || prompt.PromptTags.Any(item => item.Tag.Name.Contains(term, comparison));
    }

    private static IEnumerable<string> ParseTags(string tagsText) => tagsText
        .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(item => !string.IsNullOrWhiteSpace(item))
        .Distinct(StringComparer.CurrentCultureIgnoreCase)
        .Take(30);
}
