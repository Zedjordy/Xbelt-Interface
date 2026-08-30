using Microsoft.UI.Xaml.Controls;
using SettingsClone.Services;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using static SettingsClone.ViewModels.Field.RangeFieldViewModel;

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

    Random random = new();

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



    private string _InTokenPreview;
    public string InTokenPreview
    {
        get => _InTokenPreview;
        set
        {
            _InTokenPreview = value;
            OnPropertyChanged();
        }
    }

    private string _OutTokenPreview;
    public string OutTokenPreview
    {
        get => _OutTokenPreview;
        set
        {
            _OutTokenPreview = value;
            OnPropertyChanged();
        }
    }


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


    /*....................................................................
    @
    @
    @       Generatea sample of the selected token
    @       by using the fields it is composed by
    @
    @....................................................................*/
    private void Generate_SelectedToken(object sender)
    {

        //var type = await e.DataView.GetTextAsync();
        //var obj = (Microsoft.UI.Xaml.Controls.ListView)sender;
        StringBuilder token_string = new();
        MessageTokenViewModel selectedtoken;


        if ((sender is not SettingsClone.Views.MessageEditorView editor) ||
            (editor.DataContext is not SettingsClone.ViewModels.MessageBuilderViewModel list))
            return;



        if (editor.Name == "InEditor")
        {
            selectedtoken = list?.SelectedInToken;
            list.InTokenPreview = token_string.ToString();
        }
        else if (editor.Name == "OutEditor")
        {
            selectedtoken = list?.SelectedOutToken;
            list.OutTokenPreview = token_string.ToString();
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
                /*....................................................................
                @   
                @   case of Textfield type
                @....................................................................*/
                case TextFieldViewModel textToken:
                    token_string.Append(textToken.Value);
                    break;


                /*....................................................................
                @   
                @   case of Choice field type
                @....................................................................*/
                case ChoiceFieldViewModel choiceToken:
                    try
                    {
                        var randomOption = choiceToken.Options?[random.Next(choiceToken.Options.Count)];
                        token_string.Append(randomOption);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.WriteLine("ArgumentOutOfRangeException");
                    }
                    break;


                /*....................................................................
                @   
                @   case of Range field type
                @....................................................................*/
                case RangeFieldViewModel rangeToken:

                    switch (rangeToken.Format)
                    {
                        /*....................................................................
                        @   
                        @   case of Range.Numeric field subtype
                        @....................................................................*/
                        case RangeType.Numeric:
                            try
                            {
                                int length;

                                length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                                var randomOption = random.Next(int.Parse(rangeToken.Start), int.Parse(rangeToken.End) + 1);
                                token_string.Append(randomOption.ToString($"D{length}"));
                            }
                            catch (FormatException e)
                            {
                                Debug.WriteLine("FormatException");
                            }
                            catch (NullReferenceException e)
                            {
                                Debug.WriteLine("NullReferenceException");
                            }
                            break;
                        /*....................................................................
                        @   
                        @   case of Range.Alphanumeric field subtype
                        @....................................................................*/
                        case RangeType.AlphaNumeric:
                            {
                                StringBuilder randomOption = new();
                                int length;

                                length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                                for (int i = 0; i < length; i++)
                                {
                                    char start = rangeToken.Start[i];
                                    char end = rangeToken.End[i];

                                    randomOption.Append(GenerateRandomChar(start, end));
                                }

                                token_string.Append(randomOption);

                            }
                            break;
                        /*....................................................................
                        @   
                        @   case of Range.Alphabetic field subtype
                        @....................................................................*/
                        case RangeType.Alphabetic:
                            try
                            {
                                StringBuilder randomOption = new();
                                int length;

                                length = Math.Max(rangeToken.Start.Length, rangeToken.End.Length);

                                //A = 65
                                //Z = 90
                                //a = 97
                                //z = 122
                                for (int i = 0; i < length; i++)
                                {
                                    char start = rangeToken.Start[i];
                                    char end = rangeToken.End[i];

                                    int temp;
                                    if (start > end)
                                    {
                                        temp = start;
                                        start = end;
                                        end = (char)temp;
                                    }

                                    // between 65 - 90
                                    if ((start >= 'A' && start <= 'Z') && (end >= 'A' && end <= 'Z'))
                                    {
                                        randomOption.Append(
                                            (char)random.Next(start, end + 1));
                                    }
                                    // between 97 - 122
                                    else if ((start >= 'a' && start <= 'z') && (end >= 'a' && end <= 'z'))
                                    {
                                        randomOption.Append(
                                            (char)random.Next(start, end + 1));
                                    }
                                    else if ((start >= 'A' && start <= 'Z') &&
                                            (end >= 'a' && end <= 'z'))
                                    {
                                        List<char> options = new();

                                        options.Add((char)random.Next('a', end + 1));
                                        options.Add((char)random.Next(start, 'Z' + 1));

                                        randomOption.Append(
                                            options[random.Next(options.Count)]);
                                    }
                                    else if ((start >= 'a' && start <= 'z') &&
                                            (end >= 'A' && end <= 'Z'))
                                    {
                                        List<char> options = new();

                                        options.Add((char)random.Next(start, 'z' + 1));
                                        options.Add((char)random.Next('A', end + 1));

                                        randomOption.Append(
                                            options[random.Next(options.Count)]);
                                    }
                                    else
                                    {
                                        randomOption.Append('.');
                                    }
                                }

                                token_string.Append(randomOption);
                            }
                            catch (FormatException e)
                            {
                                Debug.WriteLine("test");
                            }
                            break;
                    }

                    break;


                /*....................................................................
                @   
                @   case of Counter field type
                @....................................................................*/
                case CounterFieldViewModel counterToken:
                    try
                    {
                        int length;

                        length = Math.Max(counterToken.Start.Length, counterToken.End.Length);


                        if (counterToken.Current > int.Parse(counterToken.End))
                        {
                            counterToken.Current = int.Parse(counterToken.Start);
                        }

                        token_string.Append((counterToken.Current++).ToString($"D{length}"));
                    }
                    catch (FormatException e)
                    {
                        Debug.WriteLine("test");
                    }



                    break;
                _:
                    break;

            }

            if (editor.Name == "InEditor")
            {
                list.InTokenPreview = token_string.ToString();
            }
            else if (editor.Name == "OutEditor")
            {
                list.OutTokenPreview = token_string.ToString();
            }
            else
            {
                return;
            }
        }
    }
    /*********************************************************************
    @
    @
    @       Given an interval return a random value
    @       0 = 48
    @       9 = 57
    @       
    @       
    @       A = 65
    @       Z = 90
    @       a = 97
    @       z = 122
    @
    @*********************************************************************/
    private char GenerateRandomChar(char start, char end)
    {

        if (IsDigit(start) && IsDigit(end))
        {
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'A' and 'Z'
        @....................................................................*/
        if (IsUpper(start) && IsUpper(end))
        {
            if (start > end)
            {
                (start, end) = (end, start);
            }
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'a' and 'z'
        @....................................................................*/
        if (IsLower(start) && IsLower(end))
        {
            if (start > end)
            {
                (start, end) = (end, start);
            }
            return (char)random.Next(start, end + 1);
        }
        /*....................................................................
        @   
        @   case of 'a' and 'Z'
        @....................................................................*/
        if (IsLower(start) && IsUpper(end))
        {
            List<char> options = new();

            options.Add((char)random.Next(start, 'z' + 1));
            options.Add((char)random.Next('A', end + 1));

            return (char)options[random.Next(options.Count)];
        }
        /*....................................................................
        @   
        @   case of 'A' and 'z'
        @....................................................................*/
        if (IsUpper(start) && IsLower(end))
        {

            List<char> options = new();

            options.Add((char)random.Next(start, 'Z' + 1));
            options.Add((char)random.Next('a', end + 1));

            return (char)options[random.Next(options.Count)];

        }
        return '.';
    }
    /*....................................................................
    @
    @
    @       Check if the char is UPPER case
    @
    @
    @....................................................................*/
    private static bool IsUpper(char c)
    {
        return c >= 'A' && c <= 'Z';
    }
    /*....................................................................
    @
    @
    @       Check if the char is lower case
    @
    @
    @....................................................................*/
    private static bool IsLower(char c)
    {
        return c >= 'a' && c <= 'z';
    }
    /*....................................................................
    @
    @
    @       Check if the char is a number digit
    @
    @
    @....................................................................*/
    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
    #endregion
}