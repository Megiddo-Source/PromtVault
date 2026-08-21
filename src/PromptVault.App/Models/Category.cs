namespace PromptVault.App.Models;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<PromptItem> Prompts { get; set; } = new List<PromptItem>();
}
