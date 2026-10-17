using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Models;

public sealed class MonitoredMessage
{
    #region PROPERTIES
    public long Sequence { get; }
    public DateTimeOffset Timestamp { get; }
    public string ScannerId { get; }
    public string ScannerName { get; }
    public MessageDirection Direction { get; }
    public string RemoteEndpoint { get; }

    private readonly byte[] _payload;

    public ReadOnlyMemory<byte> Payload => _payload;

    public int Length => _payload.Length;

    public string Hex => string.Join(
        " ",
        _payload.Select(b => b.ToString("x2")));

    // Codifica coerente con il protocollo ASCII usato nell'esempio.
    public string Text => FormatAscii(_payload);
    public enum MessageDirection
    {
        Rx,
        Tx
    }

    public string TimestampText =>
    Timestamp.ToLocalTime().ToString("HH:mm:ss.fff");

    public string DirectionText =>
        Direction == MessageDirection.Rx ? "RX" : "TX";

    #endregion

    public MonitoredMessage(
        long sequence,
        DateTimeOffset timestamp,
        string scannerId,
        string scannerName,
        MessageDirection direction,
        string remoteEndpoint,
        ReadOnlySpan<byte> payload)
    {
        Sequence = sequence;
        Timestamp = timestamp;
        ScannerId = scannerId;
        ScannerName = scannerName;
        Direction = direction;
        RemoteEndpoint = remoteEndpoint;

        // Copia i dati: il buffer UDP originale può essere riutilizzato.
        _payload = payload.ToArray();
    }

    private static string FormatAscii(ReadOnlySpan<byte> payload)
    {
        var result = new StringBuilder(payload.Length);

        foreach (byte b in payload)
        {
            if (b >= 0x20 && b <= 0x7E)
            {
                result.Append((char)b);
            }
            else
            {
                // Rende visibili STX, ETX, CR, LF e gli altri byte di controllo.
                result.Append("\\x");
                result.Append(b.ToString("X2"));
            }
        }

        return result.ToString();
    }
}
