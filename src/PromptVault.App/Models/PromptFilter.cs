namespace PromptVault.App.Models;

public enum PromptFilter
{
    All,
    Favorites,
    MostUsed,
    Recent
}

public sealed record FilterOption(PromptFilter Filter, string Label);
