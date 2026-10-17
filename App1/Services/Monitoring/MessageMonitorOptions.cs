using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Monitoring;

public sealed class MessageMonitorOptions
{
    // Numero massimo di messaggi conservati in memoria.
    public int HistoryCapacity { get; init; } = 10_000;

    // Numero massimo di messaggi in attesa di elaborazione.
    public int ChannelCapacity { get; init; } = 4_096;

    // Massimo numero di righe mostrate nella UI.
    public int MaxVisibleMessages { get; init; } = 2_000;

    // Intervallo di aggiornamento della UI.
    public int RefreshIntervalMs { get; init; } = 100;
}
