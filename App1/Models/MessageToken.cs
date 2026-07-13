using System;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SettingsClone.ViewModels;

namespace SettingsClone.Models;

public class MessageToken : ViewModelBase
{
    public Guid Id { get; set; }
    public TokenType Type { get; set; }
    public string Name { get; set; } = string.Empty;

    private uint _length = 0;
    public uint Length
    {
        get => _length;
        set
        {
            _length = Math.Max(0, value);
            OnPropertyChanged();
        }
    }

    private string _value = string.Empty;
    public string? Value 
    {
        get => _value;
        set
        {
            if(value is not null)
            {
                _value = value;
                //Length = (uint)value.Length;
                OnPropertyChanged();
            }
        }
    }
    //public override string ToString() => Name;
    public TokenType ColorToken {  get; set; }
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