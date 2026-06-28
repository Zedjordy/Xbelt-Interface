using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Models;
using SettingsClone.Services;
using SettingsClone.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace SettingsClone.ViewModels;

public class ScannerPageViewModel : ViewModelBase 
{

    private readonly INavigationService _nav;

    public ObservableCollection<ScannerSettings> Scanners { get; } = new() { new ScannerSettings() };

    public ICommand OpenSocketCommand { get; }
    public ICommand RemoveScannerCommand { get; }
    public ICommand AddScannerCommand { get; }


    #region CONSTRUCTOR

    public ScannerPageViewModel(INavigationService nav)
    {
        _nav = nav;


        OpenSocketCommand = new RelayCommand(OpenSocket);
        AddScannerCommand = new RelayCommand(AddScanner);
        RemoveScannerCommand = new RelayCommand(RemoveScanner);
    }
    #endregion



    #region METHODS
    private void AddScanner(object parameter)
    {
        Scanners.Add(new ScannerSettings());
    }

    private void RemoveScanner(object parameter)
    {
        if (parameter is not ScannerSettings scanner)
            return;

        Scanners.Remove(scanner);
    }


    //private void OpenMessageBuilder(ScannerSettings scanner)
    //{
    //    _navigationService.OpenMessageBuilder(scanner);
    //}

    private void OpenSocket(object parameter)
    {
        if (parameter is not ScannerSettings scanner)
            return;
        
        _nav.Navigate<MessageBuilderPage>(scanner);
    }

    public void Load(object parameter)
    {
        if (parameter is ObservableCollection<ScannerSettings> scanner)
        {
            //Scanners = scanner;
        }
    }

    #endregion
}