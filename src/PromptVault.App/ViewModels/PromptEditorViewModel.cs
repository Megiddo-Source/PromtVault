using System.Collections.ObjectModel;
using PromptVault.App.Models;

namespace PromptVault.App.ViewModels;

public sealed class PromptEditorViewModel : ViewModelBase
{
    private int? _id;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _content = string.Empty;
    private string _categoryName = "General";
    private string _tagsText = string.Empty;
    private string _model = "General";
    private bool _isFavorite;

    public ObservableCollection<string> CategorySuggestions { get; }
    public ObservableCollection<string> ModelSuggestions { get; } = new(new[]
    {
        "General", "ChatGPT", "Codex", "Claude", "Gemini", "DULICH", "Ollama", "Imagen"
    });

    public int? Id { get => _id; set => SetProperty(ref _id, value); }
    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    public string Content { get => _content; set => SetProperty(ref _content, value); }
    public string CategoryName { get => _categoryName; set => SetProperty(ref _categoryName, value); }
    public string TagsText { get => _tagsText; set => SetProperty(ref _tagsText, value); }
    public string Model { get => _model; set => SetProperty(ref _model, value); }
    public bool IsFavorite { get => _isFavorite; set => SetProperty(ref _isFavorite, value); }

    public PromptEditorViewModel(IEnumerable<Category> categories, PromptItem? prompt = null)
    {
        CategorySuggestions = new ObservableCollection<string>(categories.Select(item => item.Name));

        if (prompt is null)
        {
            return;
        }

        Id = prompt.Id;
        Title = prompt.Title;
        Description = prompt.Description;
        Content = prompt.Content;
        CategoryName = prompt.Category?.Name ?? "General";
        TagsText = string.Join(", ", prompt.PromptTags.Select(item => item.Tag.Name));
        Model = prompt.Model;
        IsFavorite = prompt.IsFavorite;
    }

    public bool IsValid(out string validationMessage)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            validationMessage = "El título es obligatorio.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            validationMessage = "El contenido del prompt es obligatorio.";
            return false;
        }

        validationMessage = string.Empty;
        return true;
    }

    public PromptEditData ToEditData() => new()
    {
        Id = Id,
        Title = Title,
        Description = Description,
        Content = Content,
        CategoryName = CategoryName,
        TagsText = TagsText,
        Model = Model,
        IsFavorite = IsFavorite
    };
}
