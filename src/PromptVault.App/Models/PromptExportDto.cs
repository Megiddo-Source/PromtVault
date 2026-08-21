namespace PromptVault.App.Models;

public sealed class PromptExportDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Model { get; set; } = "General";
    public bool IsFavorite { get; set; }
    public int UseCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
