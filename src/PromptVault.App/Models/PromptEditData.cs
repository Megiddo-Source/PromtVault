namespace PromptVault.App.Models;

public sealed class PromptEditData
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string CategoryName { get; set; } = "General";
    public string TagsText { get; set; } = string.Empty;
    public string Model { get; set; } = "General";
    public bool IsFavorite { get; set; }

    // Opcional: se utiliza al restaurar copias de seguridad.
    public int? UseCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
