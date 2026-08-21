using PromptVault.App.ViewModels;

namespace PromptVault.App.Services;

public interface IDialogService
{
    bool ShowPromptEditor(PromptEditorViewModel viewModel);
    IReadOnlyDictionary<string, string>? ShowVariableDialog(IReadOnlyList<string> variables);
    bool Confirm(string message, string title);
    void ShowMessage(string message, string title);
    string? ChooseImportFile();
    string? ChooseExportFile();
}
