using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SettingsClone.Models;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace SettingsClone.Views;
public sealed partial class MessageEditorView : UserControl
{
    public MessageEditorView()
    {
        this.InitializeComponent();

    }

    public event EventHandler<MessageToken> InsertTokenRequested;

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),                         //Property name
            typeof(string),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(string.Empty));    //initial value

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty AvailableTokensProperty =
        DependencyProperty.Register(
            nameof(AvailableTokens),                //Property name
            typeof(object),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(null));            //initial value

    public object AvailableTokens
    {
        get => GetValue(AvailableTokensProperty);
        set => SetValue(AvailableTokensProperty, value);
    }

    public static readonly DependencyProperty MessageTokensProperty =
        DependencyProperty.Register(
            nameof(MessageTokens),                  //Property name
            typeof(object),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(null));            //initial value

    public object MessageTokens
    {
        get => GetValue(MessageTokensProperty);
        set => SetValue(MessageTokensProperty, value);
    }

    public static readonly DependencyProperty SelectedTokenProperty =
        DependencyProperty.Register(
            nameof(SelectedToken),                  //Property name
            typeof(object),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(null));            //initial value    

    public object SelectedToken
    {
        get => GetValue(SelectedTokenProperty);
        set => SetValue(SelectedTokenProperty, value);
    }

    public static readonly DependencyProperty PreviewProperty =
        DependencyProperty.Register(
            nameof(Preview),                        //Property name
            typeof(string),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(string.Empty));    //initial value

    public string Preview
    {
        get => (string)GetValue(PreviewProperty);
        set => SetValue(PreviewProperty, value);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        // opzionale: meglio gestirlo via ICommand nel VM
    }

    private void Available_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (AvailableList.SelectedItem is MessageToken token)
        {
            InsertTokenRequested?.Invoke(this, token);
        }
    }

    // DRAG FROM MESSAGE (MOVE)
    private void Message_DragStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items[0] is MessageToken token)
        {
            e.Data.SetText(token.Name);
            e.Data.RequestedOperation = DataPackageOperation.Move;
        }
    }

    private void Message_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    //DROP WITH REAL REORDER
    private async void Message_Drop(object sender, DragEventArgs e)
    {
    }
        //    if (!e.DataView.Contains(StandardDataFormats.Text))
        //        return;

        //    var text = await e.DataView.GetTextAsync();

        //    if (!Guid.TryParse(text, out var id))
        //        return;

        //    var token = AvailableTokens.FirstOrDefault(x => x.Id == id);

        //    if (token != null)
        //    {
        //        InsertToken(token);
        //        return;
        //    }

        //    var existing = MessageTokens.FirstOrDefault(x => x.Id == id);

        //    if (existing != null)
        //    {
        //        var index = MessageTokens.IndexOf(existing);
        //        MessageTokens.Remove(existing);
        //        MessageTokens.Insert(index, existing);
        //    }
        //}



    }
