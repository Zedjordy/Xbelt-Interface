using Microsoft.UI.Xaml;
using NPOI.Util.Optional;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Text;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SettingsClone.ViewModels.MessageBuilder;

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
            if (value.Length > 0)
            {
                Length = (uint)value.Length;
            }

            if (value is null)
            {
                value = "";
            }

            SetProperty(ref _value, value);
            OnPropertyChanged();
        }
    }
    public TokenType ColorToken { get; set; }

    public ObservableCollection<MessageFieldViewModel> Fields = new();

    public MessageTokenViewModel()
    {
        this.Fields.CollectionChanged -= FieldsCollectionChanged;
        this.Fields.CollectionChanged += FieldsCollectionChanged;
    }

    private void FieldsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var act = e.Action;

        if (e.OldItems != null)
        {
            foreach (MessageFieldViewModel field in e.OldItems)
            {
                field.PropertyChanged -= OnFieldPropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (MessageFieldViewModel field in e.NewItems)
            {
                field.PropertyChanged -= OnFieldPropertyChanged;
                field.PropertyChanged += OnFieldPropertyChanged;

                if (field is ChoiceFieldViewModel choice)
                {
                    choice.Options.CollectionChanged -= OnFieldPropertyChanged;
                    choice.Options.CollectionChanged += OnFieldPropertyChanged;
                }
            }
        }
        UpdateLength();
    }


    private void OnFieldPropertyChanged(object? sender, EventArgs e)
    {
        UpdateLength();
    }


    private void UpdateLength()
    {
        uint length = 0;

        foreach (MessageFieldViewModel field in Fields)
        {
            switch (field)
            {
                case TextFieldViewModel text:
                    length += (uint)text.Value.Length;
                    break;

                case RangeFieldViewModel range:
                    if (range.Start is not null && range.End is not null)
                        length += (uint)Math.Max(range.Start.Length, range.End.Length);
                    break;

                case ChoiceFieldViewModel choice:
                    uint maxLength = 0;

                    foreach (string option in choice.Options)
                        maxLength = Math.Max(maxLength, (uint)option.Length);

                    length += maxLength;
                    break;

                case CounterFieldViewModel counter:
                    if (counter.Start is not null && counter.End is not null)
                        length += (uint)Math.Max(counter.Start.Length, counter.End.Length);
                    break;
            }
        }

        Length = length;
    }
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


