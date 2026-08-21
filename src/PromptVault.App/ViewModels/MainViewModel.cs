using System.Collections.ObjectModel;
using PromptVault.App.Models;
using PromptVault.App.Services;

namespace PromptVault.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IPromptService _promptService;
    private readonly IPromptBackupService _backupService;
    private readonly IClipboardService _clipboardService;
    private readonly IVariableResolver _variableResolver;
    private readonly IDialogService _dialogService;

    private string _searchText = string.Empty;
    private FilterOption? _selectedFilter;
    private PromptItem? _selectedPrompt;
    private string _statusText = "Preparado";
    private string _resultCountText = "0 resultados";

    public ObservableCollection<PromptItem> Prompts { get; } = new();
    public ObservableCollection<FilterOption> FilterOptions { get; } = new(new[]
    {
        new FilterOption(PromptFilter.All, "Todos"),
        new FilterOption(PromptFilter.Favorites, "Favoritos"),
        new FilterOption(PromptFilter.MostUsed, "Más usados"),
        new FilterOption(PromptFilter.Recent, "Recientes")
    });

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public FilterOption? SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value) && value is not null)
            {
                _ = LoadAsync();
            }
        }
    }

    public PromptItem? SelectedPrompt
    {
        get => _selectedPrompt;
        set
        {
            if (!SetProperty(ref _selectedPrompt, value))
            {
                return;
            }

            EditPromptCommand.RaiseCanExecuteChanged();
            DeletePromptCommand.RaiseCanExecuteChanged();
            ToggleFavoriteCommand.RaiseCanExecuteChanged();
            UsePromptCommand.RaiseCanExecuteChanged();
        }
    }

    public string StatusText { get => _statusText; private set => SetProperty(ref _statusText, value); }
    public string ResultCountText { get => _resultCountText; private set => SetProperty(ref _resultCountText, value); }

    public AsyncRelayCommand SearchCommand { get; }
    public AsyncRelayCommand NewPromptCommand { get; }
    public AsyncRelayCommand EditPromptCommand { get; }
    public AsyncRelayCommand DeletePromptCommand { get; }
    public AsyncRelayCommand ToggleFavoriteCommand { get; }
    public AsyncRelayCommand UsePromptCommand { get; }
    public AsyncRelayCommand ImportCommand { get; }
    public AsyncRelayCommand ExportCommand { get; }

    public MainViewModel(
        IPromptService promptService,
        IPromptBackupService backupService,
        IClipboardService clipboardService,
        IVariableResolver variableResolver,
        IDialogService dialogService)
    {
        _promptService = promptService;
        _backupService = backupService;
        _clipboardService = clipboardService;
        _variableResolver = variableResolver;
        _dialogService = dialogService;

        SearchCommand = new AsyncRelayCommand(() => LoadAsync());
        NewPromptCommand = new AsyncRelayCommand(NewPromptAsync);
        EditPromptCommand = new AsyncRelayCommand(EditPromptAsync, () => SelectedPrompt is not null);
        DeletePromptCommand = new AsyncRelayCommand(DeletePromptAsync, () => SelectedPrompt is not null);
        ToggleFavoriteCommand = new AsyncRelayCommand(ToggleFavoriteAsync, () => SelectedPrompt is not null);
        UsePromptCommand = new AsyncRelayCommand(UsePromptAsync, () => SelectedPrompt is not null);
        ImportCommand = new AsyncRelayCommand(ImportAsync);
        ExportCommand = new AsyncRelayCommand(ExportAsync);
    }

    public async Task InitializeAsync()
    {
        _selectedFilter = FilterOptions[0];
        OnPropertyChanged(nameof(SelectedFilter));
        await LoadAsync();
    }

    private async Task LoadAsync(int? selectId = null)
    {
        try
        {
            var selectedId = selectId ?? SelectedPrompt?.Id;
            var filter = SelectedFilter?.Filter ?? PromptFilter.All;
            var items = await _promptService.GetPromptsAsync(SearchText, filter);

            Prompts.Clear();
            foreach (var item in items)
            {
                Prompts.Add(item);
            }

            SelectedPrompt = selectedId.HasValue
                ? Prompts.FirstOrDefault(item => item.Id == selectedId.Value)
                : Prompts.FirstOrDefault();

            ResultCountText = Prompts.Count == 1 ? "1 resultado" : $"{Prompts.Count} resultados";
            StatusText = "Biblioteca actualizada";
        }
        catch (Exception exception)
        {
            StatusText = "Error al cargar la biblioteca";
            _dialogService.ShowMessage(exception.Message, "No se pudieron cargar los prompts");
        }
    }

    private async Task NewPromptAsync()
    {
        var categories = await _promptService.GetCategoriesAsync();
        var editor = new PromptEditorViewModel(categories);
        if (!_dialogService.ShowPromptEditor(editor))
        {
            return;
        }

        var id = await _promptService.SaveAsync(editor.ToEditData());
        await LoadAsync(id);
        StatusText = "Prompt creado";
    }

    private async Task EditPromptAsync()
    {
        if (SelectedPrompt is null)
        {
            return;
        }

        var fullPrompt = await _promptService.GetByIdAsync(SelectedPrompt.Id);
        if (fullPrompt is null)
        {
            return;
        }

        var categories = await _promptService.GetCategoriesAsync();
        var editor = new PromptEditorViewModel(categories, fullPrompt);
        if (!_dialogService.ShowPromptEditor(editor))
        {
            return;
        }

        var id = await _promptService.SaveAsync(editor.ToEditData());
        await LoadAsync(id);
        StatusText = "Prompt actualizado";
    }

    private async Task DeletePromptAsync()
    {
        if (SelectedPrompt is null)
        {
            return;
        }

        if (!_dialogService.Confirm($"¿Eliminar definitivamente ‘{SelectedPrompt.Title}’?", "Eliminar prompt"))
        {
            return;
        }

        await _promptService.DeleteAsync(SelectedPrompt.Id);
        await LoadAsync();
        StatusText = "Prompt eliminado";
    }

    private async Task ToggleFavoriteAsync()
    {
        if (SelectedPrompt is null)
        {
            return;
        }

        var id = SelectedPrompt.Id;
        await _promptService.ToggleFavoriteAsync(id);
        await LoadAsync(id);
        StatusText = "Favorito actualizado";
    }

    private async Task UsePromptAsync()
    {
        if (SelectedPrompt is null)
        {
            return;
        }

        var content = SelectedPrompt.Content;
        var variables = _variableResolver.ExtractVariables(content);
        if (variables.Count > 0)
        {
            var values = _dialogService.ShowVariableDialog(variables);
            if (values is null)
            {
                return;
            }
            content = _variableResolver.Resolve(content, values);
        }

        _clipboardService.SetText(content);
        var id = SelectedPrompt.Id;
        await _promptService.RecordUseAsync(id);
        await LoadAsync(id);
        StatusText = "Prompt copiado al portapapeles";
    }

    private async Task ImportAsync()
    {
        var filePath = _dialogService.ChooseImportFile();
        if (filePath is null)
        {
            return;
        }

        var count = await _backupService.ImportAsync(filePath);
        await LoadAsync();
        StatusText = $"Importados {count} prompts";
    }

    private async Task ExportAsync()
    {
        var filePath = _dialogService.ChooseExportFile();
        if (filePath is null)
        {
            return;
        }

        var count = await _backupService.ExportAsync(filePath);
        StatusText = $"Exportados {count} prompts";
        _dialogService.ShowMessage($"Se exportaron {count} prompts correctamente.", "Exportación terminada");
    }
}
