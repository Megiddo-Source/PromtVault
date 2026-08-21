using PromptVault.App.Models;

namespace PromptVault.App.Services;

public interface IPromptService
{
    Task<IReadOnlyList<PromptItem>> GetPromptsAsync(string? searchText, PromptFilter filter);
    Task<IReadOnlyList<PromptItem>> GetAllForExportAsync();
    Task<PromptItem?> GetByIdAsync(int id);
    Task<IReadOnlyList<Category>> GetCategoriesAsync();
    Task<int> SaveAsync(PromptEditData data);
    Task DeleteAsync(int id);
    Task ToggleFavoriteAsync(int id);
    Task RecordUseAsync(int id);
}
