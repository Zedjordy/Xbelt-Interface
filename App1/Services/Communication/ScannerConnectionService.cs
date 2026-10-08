using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    public class ScannerConnectionService : IConnectionService
    {
        private readonly Dictionary<Guid, IConnection> _connections = new();

        public async Task ConnectAsync(
            ScannerViewModel scanner)
        {
            if (_connections.ContainsKey(scanner.Id))
                return;

            IConnection connection;

            switch (scanner.Protocol)
            {
                case ScannerViewModel.ProtocolType.UDP:
                    connection = new UDPConnectionService(scanner);
                    break;

                case ScannerViewModel.ProtocolType.TCP:
                    connection = new TCPConnectionService(scanner);
                    break;

                default:
                    throw new NotSupportedException(
                        $"Protocollo {scanner.Protocol} non supportato.");
            }

            connection.DataReceived += Connection_DataReceived;
            connection.Error += Connection_Error;
            _connections.Add(scanner.Id, connection);

            await connection.ConnectAsync();
        }

        public async Task DisconnectAsync(ScannerViewModel scanner)
        {
            if (!_connections.TryGetValue(
                    scanner.Id,
                    out var connection))
            {
                return;
            }

            await connection.DisconnectAsync();

            connection.DataReceived -= Connection_DataReceived;
            connection.Error -= Connection_Error;

            _connections.Remove(scanner.Id);
        }

        public async Task SendAsync(ScannerViewModel scanner, byte[] data)
        {
            if (!_connections.TryGetValue(scanner.Id, out var connection))
            {
                throw new InvalidOperationException(
                    $"Lo scanner '{scanner.Name}' non è connesso.");
            }

            await connection.SendAsync(data);
        }

        private void Connection_DataReceived(object? sender, byte[] data)
        {
            StringBuilder message = new();
            string start, end;

            start = end = "";

            // pacchetti dagli scanner ricevuti
            Debug.WriteLine($"Ricevuti {data.Length} bytes");
            for (int i = 0; i < data.Length; i++)
            {
                

                if (data[i] == 2)
                {
                    start = "<STX>";
                    continue;
                }
                else if (data[i] == 3)
                {
                    end = "<ETX>";
                    continue;
                }

                char character = (char)data[i];
                message.Append(character);
            }
            Debug.WriteLine($"Ricevuta stringa {start}{message}{end}");

        }

        private void Connection_Error(object? sender, Exception exception)
        {
            Debug.WriteLine(
                $"Errore connessione: {exception.Message}");
        }
    }
}
