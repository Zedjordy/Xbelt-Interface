using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SettingsClone.ViewModels.Field;

public class ChoiceFieldViewModel : MessageFieldViewModel
{
    public ObservableCollection<string> Options { get; } = new();
    
    private string _newOption;
    public string NewOption
    {
        get => _newOption;
        set => SetProperty(ref _newOption, value);
    }

    public ICommand AddFieldOptionCommand { get; }

    public ChoiceFieldViewModel()
    {
        Type = TokenType.Choice;
        AddFieldOptionCommand = new RelayCommand(AddFieldOption);
    }

    private void AddFieldOption(object parameter)
    {
        if ((string)parameter == "")
            return;

        if (!Options.Contains((string)parameter))
        {
            Options.Add((string)parameter);
            NewOption = "";
        }
    }
}
