using Microsoft.UI.Xaml.Controls;
using SettingsClone.Services;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace SettingsClone.ViewModels;

public class MessageBuilderViewModel : ViewModelBase, INotifyPropertyChanged
{
    #region STRUCTURE

    private readonly INavigationService _nav;
    public ICommand Generate_SelectedTokenCommand { get; }
    public ObservableCollection<MessageTokenViewModel> AvailableTokens { get; } = new();
    public ObservableCollection<MessageFieldViewModel> Fields { get; } = new();
    public ObservableCollection<MessageTokenViewModel> InMessageTokens => Scanner?.InMessageTokens;
    public ObservableCollection<MessageTokenViewModel> OutMessageTokens => Scanner?.OutMessageTokens;



    private ScannerViewModel? _scanner;

    public ScannerViewModel Scanner
    {
        get => _scanner;
        set
        {
            _scanner = value;
            OnPropertyChanged();
        }
    }
    #endregion


    #region CONSTRUCTOR
    public MessageBuilderViewModel(INavigationService nav)
    {
        _nav = nav;

        //Initialize(nav);

        Seed();
        Generate_SelectedTokenCommand = new RelayCommand(Generate_SelectedToken);
    }
    #endregion


    private MessageTokenViewModel? _selectedInToken;
    public MessageTokenViewModel? SelectedInToken
    {
        get => _selectedInToken;
        set
        {
            _selectedInToken = value;
            OnPropertyChanged();
        }
    }

    private MessageTokenViewModel? _selectedOutToken;
    public MessageTokenViewModel? SelectedOutToken
    {
        get => _selectedOutToken;
        set
        {
            _selectedOutToken = value;
            OnPropertyChanged();
        }
    }

    public string InPreview =>
        Scanner == null
            ? string.Empty
            : string.Concat(Scanner.InMessageTokens.Select(t => $"<{t.Name} ({t.Length})>"));

    public string OutPreview =>
        Scanner == null
            ? string.Empty
            : string.Concat(Scanner.OutMessageTokens.Select(t => $"<{t.Name} ({t.Length})>"));


    public StringBuilder InTokenPreview { get; set; }
    public StringBuilder OutTokenPreview { get; set; }


    #region METHODS
    private void Seed()
    {
        AvailableTokens.Add(new() { Type = TokenType.STX, Name = "STX", Value = "\x02", Length = 1 });
        AvailableTokens.Add(new() { Type = TokenType.Separator, Name = "Separator" });
        AvailableTokens.Add(new() { Type = TokenType.Index, Name = "Index" });
        AvailableTokens.Add(new() { Type = TokenType.Barcode, Name = "Barcode" });
        AvailableTokens.Add(new() { Type = TokenType.Date, Name = "Date" });
        AvailableTokens.Add(new() { Type = TokenType.Time, Name = "TimeStamp" });
        AvailableTokens.Add(new() { Type = TokenType.Constant, Name = "Constant" });
        AvailableTokens.Add(new() { Type = TokenType.ETX, Name = "ETX", Value = "\x03", Length = 1 });


        Fields.Add(new TextFieldViewModel());
        Fields.Add(new ChoiceFieldViewModel());
        Fields.Add(new RangeFieldViewModel());
        Fields.Add(new CounterFieldViewModel());
    }

    public void InsertInToken(MessageTokenViewModel token)
    {
        Scanner.InMessageTokens.Add(new MessageTokenViewModel
        {
            Id = Guid.NewGuid(),
            Name = token.Name,
            Value = token.Value,
            Length = token.Length,
            Type = token.Type

        });
    }

    public void InsertOutToken(MessageTokenViewModel token)
    {
        Scanner.OutMessageTokens.Add(new MessageTokenViewModel
        {
            Id = Guid.NewGuid(),
            Name = token.Name,
            Value = token.Value,
            Length = token.Length,
            Type = token.Type
        });
    }

    public void InsertToken(MessageTokenViewModel token, int index = -1)
    {
        var clone = new MessageTokenViewModel
        {
            Type = token.Type,
            Name = token.Name,
            Value = token.Value
        };

        if (index < 0 || index >= Scanner.InMessageTokens.Count)
            Scanner.InMessageTokens.Add(clone);
        else
            Scanner.InMessageTokens.Insert(index, clone);

        OnPropertyChanged(nameof(InPreview));
    }

    public void MoveToken(MessageTokenViewModel token, int newIndex)
    {
        if (!Scanner.InMessageTokens.Contains(token))
            return;

        var oldIndex = Scanner.InMessageTokens.IndexOf(token);

        if (oldIndex == newIndex)
            return;

        InMessageTokens.Move(oldIndex, newIndex);

        OnPropertyChanged(nameof(InPreview));
    }

    public void RemoveToken(object obj, MessageTokenViewModel token)
    {
        if (obj is SettingsClone.Views.MessageEditorView editor)
        {
            if (editor.Name == "InEditor")
            {
                Scanner.InMessageTokens.Remove(token);
            }
            else
            {
                Scanner.OutMessageTokens.Remove(token);
            }
        }

    }




    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));




    public void UpdateSelectedTokenProperty(object obj, MessageTokenViewModel token)
    {
        SelectedInToken = token;
    }

    public void Initialize(object parameter)
    {
        if (parameter is ScannerViewModel scanner)
        {
            Scanner = scanner;

            //Aggiorna Preview dopo riordinamento
            scanner.InMessageTokens.CollectionChanged += Devices_CollectionChanged;
            scanner.InMessageTokens.CollectionChanged += (_, __) => OnPropertyChanged(nameof(InPreview));

            scanner.OutMessageTokens.CollectionChanged += Devices_CollectionChanged;
            scanner.OutMessageTokens.CollectionChanged += (_, __) => OnPropertyChanged(nameof(OutPreview));
        }

        OnPropertyChanged(nameof(InPreview));
        OnPropertyChanged(nameof(OutPreview));

        OnPropertyChanged(nameof(InMessageTokens));
        OnPropertyChanged(nameof(OutMessageTokens));

    }

    private void Devices_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (MessageTokenViewModel item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
    }

    private void Item_PropertyChanged(
    object? sender,
    PropertyChangedEventArgs e)
    {

        // un elemento è cambiato
        OnPropertyChanged(nameof(InPreview));
        OnPropertyChanged(nameof(OutPreview));
    }

    private void Generate_SelectedToken(object sender)
    {

        //var type = await e.DataView.GetTextAsync();
        //var obj = (Microsoft.UI.Xaml.Controls.ListView)sender;
        StringBuilder token_string = new();
        Random random = new();
        MessageTokenViewModel selectedtoken;


        if ((sender is not SettingsClone.Views.MessageEditorView editor) ||
            (editor.DataContext is not SettingsClone.ViewModels.MessageBuilderViewModel list))
            return;



        if (editor.Name == "InEditor")
        {
            selectedtoken = list?.SelectedInToken;
            list.InTokenPreview = token_string;
        }
        else if (editor.Name == "OutEditor")
        {
            selectedtoken = list?.SelectedOutToken;
            list.OutTokenPreview = token_string;
        }
        else
        {
            return;
        }

        if (selectedtoken is null)
        {
            return;
        }

        foreach (MessageFieldViewModel field in selectedtoken.Fields)
        {


            switch (field)
            {
                case TextFieldViewModel textToken:
                    token_string.Append(textToken.Value);
                    break;



                case ChoiceFieldViewModel choiceToken:

                    var randomOption = choiceToken.Options[random.Next(choiceToken.Options.Count)];
                    token_string.Append(randomOption);
                    break;



                case RangeFieldViewModel rangeToken:
                    break;



                case CounterFieldViewModel counterToken:

                    if (counterToken.Current > int.Parse(counterToken.End))
                    {
                        counterToken.Current = int.Parse(counterToken.Start);
                    }

                    token_string.Append(counterToken.Current);
                    break;
                _:
                    break;

            }
        }
    }

    #endregion
}