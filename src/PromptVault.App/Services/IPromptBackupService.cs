namespace PromptVault.App.Services;

public interface IPromptBackupService
{
    Task<int> ImportAsync(string filePath);
    Task<int> ExportAsync(string filePath);
}
