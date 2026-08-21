using System.Collections.ObjectModel;

namespace PromptVault.App.ViewModels;

public sealed class VariableDialogViewModel
{
    public ObservableCollection<VariableEntryViewModel> Values { get; }

    public VariableDialogViewModel(IEnumerable<string> variables)
    {
        Values = new ObservableCollection<VariableEntryViewModel>(
            variables.Select(name => new VariableEntryViewModel(name)));
    }
}

public sealed class VariableEntryViewModel : ViewModelBase
{
    private string _value = string.Empty;

    public VariableEntryViewModel(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public string Value { get => _value; set => SetProperty(ref _value, value); }
}
