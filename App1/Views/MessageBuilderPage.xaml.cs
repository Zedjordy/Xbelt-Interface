using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Models;
using SettingsClone.Services;
using SettingsClone.ViewModels;
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
        if (e.Items[0] is MessageToken token)
        {
            e.Data.SetText(token.Id.ToString());
            e.Data.RequestedOperation = DataPackageOperation.Copy;
        }
    }



    private void Message_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }


    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        ViewModel.Initialize(e.Parameter);
    }

}