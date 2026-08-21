using System.Windows;

namespace PromptVault.App.Views;

public partial class VariableDialogWindow : Window
{
    public VariableDialogWindow()
    {
        InitializeComponent();
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void CloseTitleBarButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
