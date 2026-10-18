using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Services;
using SettingsClone.Services.Communication;
using SettingsClone.Services.Notification;
using SettingsClone.Services.ThreadDispatcher;
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
using Windows.System;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

public partial class NetworkViewModel : IDisposable
{
    public ObservableCollection<NetworkInterface> nics { get; } = new();
    private readonly IUiDispatcher _uiDispatcher;

    public NetworkViewModel()
    {
        _uiDispatcher = App.Services.GetRequiredService<IUiDispatcher>();
        RefreshNetworkInterfaces();

        NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
    }

    private void OnNetworkAddressChanged(object? sender, EventArgs e)
    {
        _uiDispatcher.TryEnqueue(RefreshNetworkInterfaces);
    }

    private void RefreshNetworkInterfaces()
    {
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic =>
                nic.NetworkInterfaceType is NetworkInterfaceType.Wireless80211
                    or NetworkInterfaceType.Ethernet)
            .ToList();

        nics.Clear();

        foreach (var nic in interfaces)
            nics.Add(nic);
    }

    public void Dispose()
    {
        NetworkChange.NetworkAddressChanged -= OnNetworkAddressChanged;
    }
}