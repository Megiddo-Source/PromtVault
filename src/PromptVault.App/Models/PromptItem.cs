using System.ComponentModel.DataAnnotations.Schema;

namespace PromptVault.App.Models;

public sealed class PromptItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = "General";
    public bool IsFavorite { get; set; }
    public bool IsArchived { get; set; }
    public int UseCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<PromptTag> PromptTags { get; set; } = new List<PromptTag>();

    [NotMapped]
    public string TagsDisplay => PromptTags.Count == 0
        ? "—"
        : string.Join(", ", PromptTags.Select(item => item.Tag.Name).OrderBy(name => name));

    [NotMapped]
    public string FavoriteDisplay => IsFavorite ? "★" : string.Empty;

    [NotMapped]
    public string UsageSummary
    {
        get
        {
            var lastUsed = LastUsedAt.HasValue
                ? LastUsedAt.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                : "Nunca";
            return $"Usado {UseCount} veces · Último uso: {lastUsed}";
        }
    }
}
