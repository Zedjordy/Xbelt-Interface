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

public sealed partial class MessageBuilderPage : Page, INavigationAware
{
    public MessageBuilderViewModel ViewModel { get; }

    //public MessageBuilderPage(MessageBuilderViewModel vm)
    public MessageBuilderPage(MessageBuilderViewModel vm)
    {
        InitializeComponent();

        ViewModel = vm;
        DataContext = vm;

        InEditor.InsertTokenRequested += (s, token) =>
        {
            ViewModel.InsertInToken(token);
        };

        OutEditor.InsertTokenRequested += (s, token) =>
        {
            ViewModel.InsertOutToken(token);
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

    // 👇 QUESTO È IL PUNTO GIUSTO
    public void OnNavigatedTo(object? parameter)
    {
        ViewModel.Initialize(parameter);
    }

}