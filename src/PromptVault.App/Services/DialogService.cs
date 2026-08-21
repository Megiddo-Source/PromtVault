using System.Windows;
using Microsoft.Win32;
using PromptVault.App.ViewModels;
using PromptVault.App.Views;

namespace PromptVault.App.Services;

public sealed class DialogService : IDialogService
{
    private readonly Window _owner;

    public DialogService(Window owner)
    {
        _owner = owner;
    }

    public bool ShowPromptEditor(PromptEditorViewModel viewModel)
    {
        var window = new PromptEditorWindow
        {
            Owner = _owner,
            DataContext = viewModel
        };
        return window.ShowDialog() == true;
    }

    public IReadOnlyDictionary<string, string>? ShowVariableDialog(IReadOnlyList<string> variables)
    {
        var viewModel = new VariableDialogViewModel(variables);
        var window = new VariableDialogWindow
        {
            Owner = _owner,
            DataContext = viewModel
        };

        if (window.ShowDialog() != true)
        {
            return null;
        }

        return viewModel.Values.ToDictionary(item => item.Name, item => item.Value);
    }

    public bool Confirm(string message, string title) => MessageBox.Show(
        _owner,
        message,
        title,
        MessageBoxButton.YesNo,
        MessageBoxImage.Question) == MessageBoxResult.Yes;

    public void ShowMessage(string message, string title) => MessageBox.Show(
        _owner,
        message,
        title,
        MessageBoxButton.OK,
        MessageBoxImage.Information);

    public string? ChooseImportFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Importar prompts",
            Filter = "PromptVault JSON (*.json)|*.json|Todos los archivos (*.*)|*.*"
        };
        return dialog.ShowDialog(_owner) == true ? dialog.FileName : null;
    }

    public string? ChooseExportFile()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Exportar prompts",
            FileName = $"promptvault-backup-{DateTime.Now:yyyyMMdd-HHmm}.json",
            DefaultExt = ".json",
            Filter = "PromptVault JSON (*.json)|*.json"
        };
        return dialog.ShowDialog(_owner) == true ? dialog.FileName : null;
    }
}
