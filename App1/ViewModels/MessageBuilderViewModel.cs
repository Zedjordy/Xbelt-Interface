using SettingsClone.Models;
using SettingsClone.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SettingsClone.ViewModels;

public class MessageBuilderViewModel : INotifyPropertyChanged
{
    #region STRUCTURE

    private readonly INavigationService _nav;
    public ObservableCollection<MessageToken> AvailableTokens { get; } = new();

    public ObservableCollection<MessageToken> InMessageTokens => Scanner?.InMessageTokens;
    public ObservableCollection<MessageToken> OutMessageTokens => Scanner?.OutMessageTokens;



    private ScannerSettings? _scanner;

    public ScannerSettings Scanner
    {
        get => _scanner;
        set
        {
            _scanner = value;
            OnPropertyChanged();
        }
    }


    #region CONSTRUCTOR
    public MessageBuilderViewModel(INavigationService nav)
    {
        _nav = nav;

        Initialize(nav);
        Seed();
    }
    #endregion


    private MessageToken? _selectedToken;
    //private ScannerSettings? _model;
    #endregion

    public MessageToken? SelectedToken
    {
        get => _selectedToken;
        set
        {
            _selectedToken = value;
            OnPropertyChanged();
        }
    }

    public string InPreview =>
        Scanner == null
            ? string.Empty
            : string.Concat(Scanner.InMessageTokens.Select(t => $"<{t.Name}>"));

    public string OutPreview =>
        Scanner == null
            ? string.Empty
            : string.Concat(Scanner.OutMessageTokens.Select(t => $"<{t.Name}>"));



    #region METHODS
    private void Seed()
    {
        AvailableTokens.Add(new() { Type = TokenType.STX, Name = "STX", Value="\x02", Length = 1});
        AvailableTokens.Add(new() { Type = TokenType.Separator, Name = "Separator" });
        AvailableTokens.Add(new() { Type = TokenType.Index, Name = "Index" });
        AvailableTokens.Add(new() { Type = TokenType.Barcode, Name = "Barcode" });
        AvailableTokens.Add(new() { Type = TokenType.Date, Name = "Date" });
        AvailableTokens.Add(new() { Type = TokenType.Time, Name = "TimeStamp" });
        AvailableTokens.Add(new() { Type = TokenType.Constant, Name = "Constant" });
        AvailableTokens.Add(new() { Type = TokenType.ETX, Name = "ETX", Value = "\x03", Length = 1});
    }

    public void InsertInToken(MessageToken token)
    {
        Scanner.InMessageTokens.Add(token);
    }

    public void InsertOutToken(MessageToken token)
    {
        Scanner.OutMessageTokens.Add(token);
    }

    public void InsertToken(MessageToken token, int index = -1)
    {
        var clone = new MessageToken
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

    public void MoveToken(MessageToken token, int newIndex)
    {
        if (!Scanner.InMessageTokens.Contains(token))
            return;

        var oldIndex = Scanner.InMessageTokens.IndexOf(token);

        if (oldIndex == newIndex)
            return;

        InMessageTokens.Move(oldIndex, newIndex);

        OnPropertyChanged(nameof(InPreview));
    }

    public void RemoveToken(object obj, MessageToken token)
    {
        if(obj is SettingsClone.Views.MessageEditorView editor)
        {
            if(editor.Name == "InEditor")
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


    public void Initialize(object parameter)
    {
        if (parameter is ScannerSettings scanner)
        {
            _scanner = scanner;

            //Aggiorna Preview dopo riordinamento
            scanner.InMessageTokens.CollectionChanged += (_, __) => OnPropertyChanged(nameof(InPreview));
            scanner.OutMessageTokens.CollectionChanged += (_, __) => OnPropertyChanged(nameof(OutPreview));
        }

        OnPropertyChanged(nameof(InPreview));
        OnPropertyChanged(nameof(OutPreview));

        OnPropertyChanged(nameof(InMessageTokens));
        OnPropertyChanged(nameof(OutMessageTokens));
    }
    #endregion
}