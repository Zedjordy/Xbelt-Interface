using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using SettingsClone.ViewModels.Field;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace SettingsClone.Views;

public sealed partial class MessageBuilderPage : Page
{
    public MessageBuilderViewModel ViewModel { get; }

    public MessageBuilderPage()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<MessageBuilderViewModel>();
        DataContext = ViewModel;

        InEditor.InsertTokenRequested += (s, token) =>
        {
            ViewModel.InsertInToken(token);
        };

        OutEditor.InsertTokenRequested += (s, token) =>
        {
            ViewModel.InsertOutToken(token);
        };

        InEditor.RemoveTokenRequested += (s, token) =>
        {
            ViewModel.RemoveToken(s, token);
        };

        OutEditor.RemoveTokenRequested += (s, token) =>
        {
            ViewModel.RemoveToken(s, token);
        };
    }

    // DRAG FROM AVAILABLE
    private void Available_DragStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items[0] is MessageTokenViewModel token)
        {
            e.Data.SetText(token.Id.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Copy;
        }
    }

    private void Message_DragStarting(object sender, DragItemsStartingEventArgs e)
    {
        if (e.Items[0] is MessageFieldViewModel token)
        {
            e.Data.SetText(token.Type.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Copy;
        }
    }

    private void Message_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
    }

    //DROP WITH REAL REORDER
    private async void Message_Drop(object sender, DragEventArgs e)
    {
        var type = await e.DataView.GetTextAsync();
        var obj = (Microsoft.UI.Xaml.Controls.ListView)sender;
        
        MessageTokenViewModel selectedtoken;


        if (obj.DataContext is not SettingsClone.Views.MessageEditorView editor)
            return;

        if(editor.DataContext is not SettingsClone.ViewModels.MessageBuilderViewModel list)
            return;

        

        if (editor.Name == "InEditor")
        {
            selectedtoken = list?.SelectedInToken;
        }
        else if(editor.Name == "OutEditor")
        {
            selectedtoken = list?.SelectedOutToken;
        }             
        else
        {
            return;
        }

        if (selectedtoken is null)
        {
            return;
        }

        switch (type)
        {
            case "Text":
                selectedtoken.Fields.Add(new TextFieldViewModel());
                break;

            case "Range":
                selectedtoken.Fields.Add(new RangeFieldViewModel());
                break;

            case "Choice":
                selectedtoken.Fields.Add(new ChoiceFieldViewModel());
                break;

            case "Counter":
                selectedtoken.Fields.Add(new CounterFieldViewModel());
                break;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ViewModel.Initialize(e.Parameter);
    }

}