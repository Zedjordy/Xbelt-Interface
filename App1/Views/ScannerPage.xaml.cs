using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SettingsClone.Models;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class ScannerPage : Page
{
    public ScannerPageViewModel ViewModel { get; }


    public ScannerPage(ScannerPageViewModel vm)
    {
        InitializeComponent();

        ViewModel = vm;
        DataContext = vm;
    }


    public void OnNavigatedTo(object? parameter)
    {
        //if (parameter is ObservableCollection<ScannerSettings> settings)
        //{
        //    ViewModel.Load(settings);
        //}
    }

}