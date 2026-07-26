using Microsoft.UI.Xaml;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SettingsClone.ViewModels;

public class MessageTokenViewModel : ViewModelBase
{


    public Guid Id { get; set; }
    public TokenType Type { get; set; }
    public string Name { get; set; } = string.Empty;

    private uint _length;
    public uint Length
    {
        get => _length;
        set
        {
            if (uint.TryParse(value.ToString(), out uint result))
            {
                _length = value;
                OnPropertyChanged();
            }
            else
            {
                _length = 0;
            }
        }
    }

    private string _value = string.Empty;
    public string? Value
    {
        get => _value;
        set
        {
            if (value is not null)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
    public TokenType ColorToken { get; set; }

    public ObservableCollection<MessageFieldViewModel> Fields = new();
    
}

public enum TokenType
{
    Barcode,
    Index,
    STX,
    ETX,
    Separator,
    Constant,
    Date,
    Time
}
