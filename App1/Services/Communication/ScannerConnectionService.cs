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
        //event EventHandler<StringBuilder>? Notify_response;

        /*....................................................................
        @
        @   OpenConnectionAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task OpenConnectionAsync(ScannerViewModel scanner)
        {
            if (_connections.ContainsKey(scanner.Id))
                return;

            IConnection connection;

            /*Create connection according to the configured one*/
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

            /*Event Subscription*/
            connection.DataReceived += scanner.OnDataReceived;
            connection.Error += Connection_Error;
            scanner.Notify_response += scanner.Elaborate_response;


            /*Add connection in list*/
            _connections.Add(scanner.Id, connection);

            await connection.ConnectAsync();
        }

        /*....................................................................
        @
        @   CloseConnectionAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task CloseConnectionAsync(ScannerViewModel scanner)
        {
            if (!_connections.TryGetValue(
                    scanner.Id,
                    out var connection))
            {
                return;
            }

            await connection.DisconnectAsync();

            connection.DataReceived -= scanner.OnDataReceived;            
            connection.Error -= Connection_Error;
            scanner.Notify_response -= scanner.Elaborate_response;

            _connections.Remove(scanner.Id);
        }


        /*....................................................................
        @
        @   SendAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task SendAsync(ScannerViewModel scanner, byte[] data)
        {
            if (!_connections.TryGetValue(scanner.Id, out var connection))
            {
                throw new InvalidOperationException(
                    $"Lo scanner '{scanner.Name}' non è connesso.");
            }

            //await connection.SendAsync(data);
        }

        /*....................................................................
        @
        @   Connection_Error
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        private void Connection_Error(object? sender, Exception exception)
        {
            Debug.WriteLine(
                $"Errore connessione: {exception.Message}");
        }
    }
}
