using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Services;
using SettingsClone.Services.Communication;
using SettingsClone.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Windows.Input;


namespace SettingsClone.ViewModels;

public class ScannerPageViewModel : ViewModelBase
{

    private readonly INavigationService _nav;
    private readonly IConnectionService _connectionService;
    public ObservableCollection<NetworkInterface> NIC { get; } = new(NetworkInterface.GetAllNetworkInterfaces()
        .Where(nic => /*nic.OperationalStatus == OperationalStatus.Up
                      &&*/ (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                      || nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)));

    public ObservableCollection<ScannerViewModel> Scanners { get; } = new() { new ScannerViewModel() };


    //Single scanner command
    public ICommand OpenSocketCommand { get; }

    //Scanner page commands
    public ICommand RemoveScannerCommand { get; }
    public ICommand AddScannerCommand { get; }

    //Network commands
    public ICommand ConnectCommand { get; }
    public ICommand DisconnectCommand { get; }
    public ICommand SendCommand { get; }


    #region CONSTRUCTOR

    public ScannerPageViewModel(
        INavigationService nav,
        IConnectionService connectionService)
    {
        _nav = nav;

        OpenSocketCommand = new RelayCommand(OpenSocket);
        AddScannerCommand = new RelayCommand(AddScanner);
        RemoveScannerCommand = new RelayCommand(RemoveScanner);

        _connectionService = connectionService;

        ConnectCommand =
            new RelayCommand(async p =>
            {
                if (p is ScannerViewModel scanner)
                    await _connectionService.ConnectAsync(scanner);
            });

        DisconnectCommand =
            new RelayCommand(async p =>
            {
                if (p is ScannerViewModel scanner)
                    await _connectionService.DisconnectAsync(scanner);
            });

    }
    #endregion



    #region METHODS
    private void AddScanner(object parameter)
    {
        Scanners.Add(new ScannerViewModel());
    }

    private void RemoveScanner(object parameter)
    {
        if (parameter is not ScannerViewModel scanner)
            return;

        Scanners.Remove(scanner);
    }


    private void OpenSocket(object parameter)
    {
        if (parameter is not ScannerViewModel scanner)
            return;

        _nav.Navigate<MessageBuilderPage>(scanner);
    }

    public async void Check_Connection(object sender)
    {
        if (sender is not ToggleSwitch toggle)
            return;

        if (toggle.IsOn)
        {
            await _connectionService.ConnectAsync((ScannerViewModel)toggle.DataContext);
        }
        else
        {
            await _connectionService.DisconnectAsync((ScannerViewModel)toggle.DataContext);
        }
    }

    private ObservableCollection<NetworkInterface> Lookup_NIC()
    {
        ObservableCollection<NetworkInterface> _nic = new();

        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            _nic.Add(ni);
            Debug.WriteLine(
                $"NIC: {ni.Name} - {ni.OperationalStatus}");

            foreach (var address in ni.GetIPProperties().UnicastAddresses)
            {
                Debug.WriteLine(
                    $"    IP: {address.Address} " +
                    $"Family: {address.Address.AddressFamily}");
            }
        }
        return _nic;
    }

    public void GetIps(object sender)
    {
        ObservableCollection<IPAddress> ipProps = new();

        // foreach (var iface in NIC)
        //{

        ipProps.Clear();


        if (sender is not ComboBox CBox)
            return;

        if (CBox.SelectedItem is not NetworkInterface iface)
            return;

        if (CBox.DataContext is not ScannerViewModel scanner)
            return;


        var _ipProps = iface.GetIPProperties();

        // Get IPv4 addresses (skip IPv6)
        foreach (var ipAddr in _ipProps.UnicastAddresses)
        {
            if (ipAddr.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4
            {
                ipProps.Add(ipAddr.Address);
                Console.WriteLine($"Interface: {iface.Name}, IP: {ipAddr.Address}");
            }
        }

        scanner.IPlist = ipProps;
        scanner.SubnetMask = "0.0.0.0";
        scanner.Gateway= "0.0.0.0";

        // }
    }

    public void GetIpInfo(object sender)
    {
        if (sender is not ComboBox CBox)
            return;

        if (CBox.SelectedItem is not IPAddress iface)
            return;

        if (CBox.DataContext is not ScannerViewModel scanner)
            return;

        var nic = scanner.Net_Interface;

        if (nic != null)
        {
            var ipInfo = nic.GetIPProperties()
                .UnicastAddresses
                .First(a => a.Address.Equals(scanner.selected_Ip));

            scanner.SubnetMask = ipInfo.IPv4Mask.ToString();

            var gateway = nic.GetIPProperties()
                .GatewayAddresses
                .FirstOrDefault(g =>
                    g.Address.AddressFamily == AddressFamily.InterNetwork)
                ?.Address;


            scanner.Gateway = gateway?.ToString() ?? "0.0.0.0";
        }
    }
    #endregion
}