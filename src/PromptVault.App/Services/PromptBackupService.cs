using System.Text.Json;
using PromptVault.App.Models;

namespace PromptVault.App.Services;

public sealed class PromptBackupService : IPromptBackupService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly IPromptService _promptService;

    public PromptBackupService(IPromptService promptService)
    {
        _promptService = promptService;
    }

    public async Task<int> ExportAsync(string filePath)
    {
        var prompts = await _promptService.GetAllForExportAsync();
        var exportItems = prompts.Select(item => new PromptExportDto
        {
            Title = item.Title,
            Description = item.Description,
            Content = item.Content,
            Category = item.Category?.Name ?? string.Empty,
            Tags = item.PromptTags.Select(link => link.Tag.Name).OrderBy(name => name).ToList(),
            Model = item.Model,
            IsFavorite = item.IsFavorite,
            UseCount = item.UseCount,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            LastUsedAt = item.LastUsedAt
        }).ToList();

        await using var stream = System.IO.File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, exportItems, JsonOptions);
        return exportItems.Count;
    }

    public async Task<int> ImportAsync(string filePath)
    {
        await using var stream = System.IO.File.OpenRead(filePath);
        var items = await JsonSerializer.DeserializeAsync<List<PromptExportDto>>(stream, JsonOptions)
                    ?? new List<PromptExportDto>();

        var imported = 0;
        foreach (var item in items.Where(item => !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Content)))
        {
            await _promptService.SaveAsync(new PromptEditData
            {
                Title = item.Title,
                Description = item.Description,
                Content = item.Content,
                CategoryName = item.Category,
                TagsText = string.Join(", ", item.Tags),
                Model = item.Model,
                IsFavorite = item.IsFavorite,
                UseCount = item.UseCount,
                CreatedAt = item.CreatedAt == default ? null : item.CreatedAt,
                UpdatedAt = item.UpdatedAt == default ? null : item.UpdatedAt,
                LastUsedAt = item.LastUsedAt
            });
            imported += 1;
        }

        return imported;
    }
}
