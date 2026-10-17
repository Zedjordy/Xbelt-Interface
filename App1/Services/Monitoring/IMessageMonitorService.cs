using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsClone.Models;
using static SettingsClone.Models.MonitoredMessage;

namespace SettingsClone.Services.Monitoring;

public interface IMessageMonitorService
{
    bool TryPublish(        
        string scannerId,
        string scannerName,
        MessageDirection direction,
        string remoteEndpoint,
        ReadOnlySpan<byte> payload);

    IReadOnlyList<MonitoredMessage> GetSnapshot();

    long DroppedMessages { get; }

    long EvictedMessages { get; }

    long TotalPublished { get; }
}
