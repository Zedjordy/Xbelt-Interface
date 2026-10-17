using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using SettingsClone.Models;
using System.Threading.Tasks;
using static SettingsClone.Models.MonitoredMessage;

namespace SettingsClone.Services.Monitoring;

class MessageMonitorService : BackgroundService, IMessageMonitorService
{
    private readonly Channel<MonitoredMessage> _channel;
    private readonly MessageMonitorOptions _options;

    private readonly object _publishLock = new();
    private readonly object _historyLock = new();

    private readonly MonitoredMessage?[] _history;
    private int _nextIndex;
    private int _historyCount;

    private long _sequence;
    private long _droppedMessages;
    private long _evictedMessages;
    private long _totalPublished;
    private int _accepting = 1;

    public long DroppedMessages =>
        Interlocked.Read(ref _droppedMessages);

    public long EvictedMessages =>
        Interlocked.Read(ref _evictedMessages);

    public long TotalPublished =>
        Interlocked.Read(ref _totalPublished);

    public MessageMonitorService(MessageMonitorOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.HistoryCapacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(options.HistoryCapacity));

        if (options.ChannelCapacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(options.ChannelCapacity));

        _options = options;
        _history = new MonitoredMessage[options.HistoryCapacity];

        _channel = Channel.CreateBounded<MonitoredMessage>(
            new BoundedChannelOptions(options.ChannelCapacity)
            {
                SingleReader = true,
                SingleWriter = false,

                // TryWrite fallisce quando il canale è pieno.
                // Il producer non aspetta che si liberi spazio.
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public bool TryPublish(
        string scannerId,
        string scannerName,
        MessageDirection direction,
        string remoteEndpoint,
        ReadOnlySpan<byte> payload)
    {
        if (Volatile.Read(ref _accepting) == 0)
            return false;

        lock (_publishLock)
        {
            if (Volatile.Read(ref _accepting) == 0)
                return false;

            long sequence = _sequence + 1;

            var message = new MonitoredMessage(
                sequence,
                DateTimeOffset.UtcNow,
                scannerId,
                scannerName,
                direction,
                remoteEndpoint,
                payload);

            if (!_channel.Writer.TryWrite(message))
            {
                Interlocked.Increment(ref _droppedMessages);
                return false;
            }

            _sequence = sequence;
            Interlocked.Increment(ref _totalPublished);

            return true;
        }
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            await foreach (
                MonitoredMessage message in
                _channel.Reader.ReadAllAsync(stoppingToken))
            {
                AddToHistory(message);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Arresto previsto dell'applicazione.
        }
    }

    private void AddToHistory(MonitoredMessage message)
    {
        lock (_historyLock)
        {
            if (_historyCount == _history.Length)
            {
                Interlocked.Increment(ref _evictedMessages);
            }
            else
            {
                _historyCount++;
            }

            _history[_nextIndex] = message;
            _nextIndex = (_nextIndex + 1) % _history.Length;
        }
    }

    public IReadOnlyList<MonitoredMessage> GetSnapshot()
    {
        lock (_historyLock)
        {
            var snapshot = new MonitoredMessage[_historyCount];

            int oldestIndex =
                (_nextIndex - _historyCount + _history.Length)
                % _history.Length;

            for (int i = 0; i < _historyCount; i++)
            {
                snapshot[i] = _history[
                    (oldestIndex + i) % _history.Length]!;
            }

            return snapshot;
        }
    }

    public override void Dispose()
    {
        Interlocked.Exchange(ref _accepting, 0);
        _channel.Writer.TryComplete();

        base.Dispose();
    }
}
