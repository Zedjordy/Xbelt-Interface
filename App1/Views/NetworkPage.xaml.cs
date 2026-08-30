using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class NetworkPage : Page
{
    public NetworkPageViewModel ViewModel { get; }


    public NetworkPage()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<NetworkPageViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        //var param = (ScannerPageViewModel)e.Parameter;
        ViewModel.Initialize(e.Parameter);
    }
}