namespace PromptVault.App.Models;

public sealed class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<PromptTag> PromptTags { get; set; } = new List<PromptTag>();
}
