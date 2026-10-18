using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Services;
using SettingsClone.Services.Monitoring;
using SettingsClone.Views;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SettingsClone.Models;
using static SettingsClone.Models.MonitoredMessage;
using SettingsClone.ViewModels.Scanner;

namespace SettingsClone.ViewModels.Monitor;

public partial class MessageMonitorViewModel : ObservableObject, IDisposable
{
    private readonly INavigationService _nav;
    public ObservableCollection<ScannerViewModel> Scanners { get; set; }
    private const int listenPort = 11000;


    private readonly IMessageMonitorService _monitor;
    private readonly DispatcherQueueTimer _timer;
    private readonly int _maxVisibleMessages;

    private long _lastSequence;
    private bool _disposed;

    public ObservableCollection<MonitoredMessage> Messages { get; } = new();

    public ObservableCollection<string> DirectionOptions { get; } =
        new() { "All", "RX", "TX" };

    public ObservableCollection<string> ScannerFilters { get; } =
        new() { "All" };

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _directionFilter = "All";

    [ObservableProperty]
    private string _scannerFilter = "All";

    public long DroppedMessages => _monitor.DroppedMessages;

    public long EvictedMessages => _monitor.EvictedMessages;

    public long TotalPublished => _monitor.TotalPublished;


    #region CONSTRUCTOR

    public MessageMonitorViewModel(
        INavigationService nav,
        IMessageMonitorService monitor,
        MessageMonitorOptions options)
    {
        _nav = nav;
        _monitor = monitor;
        _maxVisibleMessages = options.MaxVisibleMessages;

        // Il ViewModel deve essere creato sul thread UI.
        DispatcherQueue dispatcher =
            DispatcherQueue.GetForCurrentThread()
            ?? throw new InvalidOperationException(
                "Il ViewModel deve essere creato sul thread UI.");

        _timer = dispatcher.CreateTimer();
        _timer.Interval =
            TimeSpan.FromMilliseconds(options.RefreshIntervalMs);

        _timer.Tick += OnTimerTick;

        // Mostra lo storico già presente all'apertura.
        RebuildVisibleMessages();

        _timer.Start();
    }
    #endregion

    /*....................................................................
    @
    @
    @                            INITIALIZE
    @
    @
    @....................................................................*/
    public void Initialize(object parameter)
    {
        if (parameter is ScannerPageViewModel scanner)
        {
            Scanners = scanner.Scanners;
        }

    }
    /*....................................................................
    @   end of Initialize
    @....................................................................*/

    private void OnTimerTick(
       DispatcherQueueTimer sender,
       object args)
    {
        if (_disposed)
            return;

        IReadOnlyList<MonitoredMessage> snapshot =
            _monitor.GetSnapshot();

        RefreshScannerFilters(snapshot);

        foreach (MonitoredMessage message in snapshot)
        {
            if (message.Sequence <= _lastSequence)
                continue;

            _lastSequence = message.Sequence;

            if (MatchesFilters(message))
                AddVisibleMessage(message);
        }

        OnPropertyChanged(nameof(DroppedMessages));
        OnPropertyChanged(nameof(EvictedMessages));
        OnPropertyChanged(nameof(TotalPublished));
    }

    partial void OnSearchTextChanged(string value) =>
        RebuildVisibleMessages();

    partial void OnDirectionFilterChanged(string value) =>
        RebuildVisibleMessages();

    partial void OnScannerFilterChanged(string value) =>
        RebuildVisibleMessages();

    private bool MatchesFilters(MonitoredMessage message)
    {
        if (DirectionFilter == "RX" &&
            message.Direction != MessageDirection.Rx)
            return false;

        if (DirectionFilter == "TX" &&
            message.Direction != MessageDirection.Tx)
            return false;

        if (ScannerFilter != "All" &&
            message.ScannerId != ScannerFilter)
            return false;

        string search = SearchText.Trim();

        if (search.Length == 0)
            return true;

        return message.ScannerName.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase)
            || message.ScannerId.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase)
            || message.Hex.Contains(
                   search.Replace(" ", ""),
                   StringComparison.OrdinalIgnoreCase)
            || message.Text.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase);
    }

    private void AddVisibleMessage(MonitoredMessage message)
    {
        Messages.Add(message);

        while (Messages.Count > _maxVisibleMessages)
            Messages.RemoveAt(0);
    }

    private void RebuildVisibleMessages()
    {
        if (_disposed)
            return;

        IReadOnlyList<MonitoredMessage> snapshot =
            _monitor.GetSnapshot();

        RefreshScannerFilters(snapshot);

        Messages.Clear();

        // Mantieni soltanto gli ultimi N risultati che rispettano i filtri.
        foreach (MonitoredMessage message in snapshot)
        {
            if (MatchesFilters(message))
                AddVisibleMessage(message);
        }

        if (snapshot.Count > 0)
            _lastSequence = Math.Max(
                _lastSequence,
                snapshot[^1].Sequence);
    }

    private void RefreshScannerFilters(
        IReadOnlyList<MonitoredMessage> snapshot)
    {
        foreach (string scannerId in snapshot
                     .Select(m => m.ScannerId)
                     .Distinct(StringComparer.Ordinal))
        {
            if (!ScannerFilters.Contains(scannerId))
                ScannerFilters.Add(scannerId);
        }
    }

    [RelayCommand]
    private void ClearView()
    {
        Messages.Clear();

        // Pulisce solo la vista: lo storico del servizio rimane intatto.
        IReadOnlyList<MonitoredMessage> snapshot =
            _monitor.GetSnapshot();

        if (snapshot.Count > 0)
            _lastSequence = snapshot[^1].Sequence;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _timer.Stop();
        _timer.Tick -= OnTimerTick;
    }

    public void StartMonitoring()
    {
        if (_timer.IsRunning)
            return;

        RebuildVisibleMessages();
        _timer.Start();
    }

    public void StopMonitoring()
    {
        _timer.Stop();
    }
}
