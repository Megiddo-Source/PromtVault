using System.Windows;
using PromptVault.App.ViewModels;

namespace PromptVault.App.Views;

public partial class PromptEditorWindow : Window
{
    private const string MaximizeGlyph = "\uE922";
    private const string RestoreGlyph = "\uE923";

    public PromptEditorWindow()
    {
        InitializeComponent();
        UpdateMaximizeButton();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not PromptEditorViewModel viewModel)
        {
            return;
        }

        if (!viewModel.IsValid(out var validationMessage))
        {
            MessageBox.Show(this, validationMessage, "Revisa el prompt", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(this);
        }
        else
        {
            SystemCommands.MaximizeWindow(this);
        }
    }

    private void CloseTitleBarButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        UpdateMaximizeButton();
    }

    private void UpdateMaximizeButton()
    {
        if (MaximizeButton is null)
        {
            return;
        }

        MaximizeButton.Content = WindowState == WindowState.Maximized ? RestoreGlyph : MaximizeGlyph;
        MaximizeButton.ToolTip = WindowState == WindowState.Maximized ? "Restaurar" : "Maximizar";
    }
}
