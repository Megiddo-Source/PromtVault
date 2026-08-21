using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using PromptVault.App.Data;
using PromptVault.App.Services;
using PromptVault.App.ViewModels;

namespace PromptVault.App;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PromptVault");
            Directory.CreateDirectory(appFolder);

            var databasePath = Path.Combine(appFolder, "promptvault.db");
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            Func<AppDbContext> dbFactory = () => new AppDbContext(options);

            await DbInitializer.InitializeAsync(dbFactory);

            var promptService = new PromptService(dbFactory);
            var backupService = new PromptBackupService(promptService);
            var clipboardService = new ClipboardService();
            var variableResolver = new VariableResolver();

            var window = new MainWindow();
            var dialogService = new DialogService(window);
            var viewModel = new MainViewModel(
                promptService,
                backupService,
                clipboardService,
                variableResolver,
                dialogService);

            window.DataContext = viewModel;
            MainWindow = window;
            window.Show();

            await viewModel.InitializeAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"No se pudo iniciar PromptVault.\n\n{exception.Message}",
                "Error de inicio",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
        }
    }
}
