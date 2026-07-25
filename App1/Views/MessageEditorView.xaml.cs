using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SettingsClone.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace SettingsClone.Views;
public sealed partial class MessageEditorView : UserControl
{
    public MessageEditorView()
    {
        this.InitializeComponent();

    }

    public event EventHandler<MessageTokenViewModel> InsertTokenRequested;
    public event EventHandler<MessageTokenViewModel> RemoveTokenRequested;

    /*............................................................................*/
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
    /*............................................................................*/


    /*............................................................................*/
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
    /*............................................................................*/


    /*............................................................................*/
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
    /*............................................................................*/


    /*............................................................................*/
    public static readonly DependencyProperty SelectedTokenProperty =
        DependencyProperty.Register(
            nameof(SelectedToken),                  //Property name
            typeof(MessageTokenViewModel),                         //types accepted
            typeof(MessageEditorView),              //Property owner
            new PropertyMetadata(null, OnSelectedTokenChanged));            //initial value    

    public MessageTokenViewModel SelectedToken
    {
        get => (MessageTokenViewModel)GetValue(SelectedTokenProperty);
        //get => (MessageToken)MessageList.SelectedItem;
        //get => GetValue(SelectedTokenProperty);
        set => SetValue(SelectedTokenProperty, value);
    }
    /*............................................................................*/


    /*............................................................................*/

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
    /*............................................................................*/



    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (MessageList.SelectedItem is MessageTokenViewModel token)
        {
            RemoveTokenRequested?.Invoke(this, token);
        }
    }

    private void Available_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (AvailableList.SelectedItem is MessageTokenViewModel token)
        {
            InsertTokenRequested?.Invoke(this, token);
        }
    }

    private void SelectedInToken(object sender, RoutedEventArgs e)
    {
    }

    // DRAG FROM MESSAGE (MOVE)
    private void Message_DragStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items[0] is MessageTokenViewModel token)
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

    private static void OnSelectedTokenChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
    {
        var control = (MessageEditorView)d;

        var newValue = e.NewValue as MessageTokenViewModel;

        // sincronizza UI interna se serve
        control.MessageList.SelectedItem = newValue;

        // opzionale: reset visual state se null
        if (newValue == null)
        {
            control.MessageList.SelectedItem = new MessageTokenViewModel();
        }
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (args.NewValue != sender.Value)
            return;
    }
}
