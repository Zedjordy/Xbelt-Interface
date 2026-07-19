using SettingsClone.Models;
using SettingsClone.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    #endregion


    #region CONSTRUCTOR
    public MessageBuilderViewModel(INavigationService nav)
    {
        _nav = nav;

        //Initialize(nav);
        Seed();
    }
    #endregion


    private MessageToken? _selectedInToken;
    public MessageToken? SelectedInToken
    {
        get => _selectedInToken;
        set
        {            
            _selectedInToken = value;
            OnPropertyChanged();
        }
    }

    private MessageToken? _selectedOutToken;
    public MessageToken? SelectedOutToken
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
        Scanner.InMessageTokens.Add(new MessageToken
        {
            Id = Guid.NewGuid(),
            Name = token.Name,
            Value = token.Value,
            Length = token.Length,
            Type = token.Type
            
        });
    }

    public void InsertOutToken(MessageToken token)
    {
        Scanner.OutMessageTokens.Add(new MessageToken
        {
            Id = Guid.NewGuid(),
            Name = token.Name,
            Value = token.Value,
            Length = token.Length,
            Type = token.Type
        });
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

    public void UpdateSelectedTokenProperty(object obj, MessageToken token)
    {
        SelectedInToken = token;
    }

    public void Initialize(object parameter)
    {
        if (parameter is ScannerSettings scanner)
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
            foreach (MessageToken item in e.NewItems)
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
    #endregion
}