using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Models;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class ScannerPage : Page
{
    public ScannerPageViewModel ViewModel { get; }


    public ScannerPage()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<ScannerPageViewModel>();
        DataContext = ViewModel;
    }


protected override void OnNavigatedTo(NavigationEventArgs e)
{
    base.OnNavigatedTo(e);

    var param = (string)e.Parameter;
}

}