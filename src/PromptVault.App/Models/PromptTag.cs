namespace PromptVault.App.Models;

public sealed class PromptTag
{
    public int PromptItemId { get; set; }
    public PromptItem PromptItem { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
