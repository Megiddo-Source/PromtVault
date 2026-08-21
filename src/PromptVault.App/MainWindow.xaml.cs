using System.Windows;
using System.Windows.Input;

namespace PromptVault.App;

public partial class MainWindow : Window
{
    private const string MaximizeGlyph = "\uE922";
    private const string RestoreGlyph = "\uE923";

    public MainWindow()
    {
        InitializeComponent();
        UpdateMaximizeButton();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is ViewModels.MainViewModel viewModel)
        {
            viewModel.SearchCommand.Execute(null);
            e.Handled = true;
        }
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

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
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
