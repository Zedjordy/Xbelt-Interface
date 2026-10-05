using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    public class TCPConnectionService : IConnection
    {
        private readonly ScannerViewModel _settings;

        private TcpClient? _client;
        private NetworkStream? _stream;

        private CancellationTokenSource? _cts;

        public bool IsConnected { get; private set; }

        public event EventHandler<byte[]>? DataReceived;
        public event EventHandler<Exception>? Error;

        public TCPConnectionService(ScannerViewModel settings)
        {
            _settings = settings;
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            _client = new TcpClient();

            await _client.ConnectAsync(
                _settings.IpAddress,
                _settings.ServerPort);

            _stream = _client.GetStream();

            _cts = new CancellationTokenSource();

            IsConnected = true;

            _ = ReceiveLoopAsync(_cts.Token);
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await _stream!.ReadAsync(
                        buffer,
                        cancellationToken);

                    if (bytesRead == 0)
                        break;

                    byte[] data = buffer[..bytesRead];

                    DataReceived?.Invoke(this, data);
                }
            }
            catch (OperationCanceledException)
            {
                // Disconnect normale
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, ex);
            }
        }

        public async Task SendAsync(byte[] data)
        {
            if (_stream == null)
                throw new InvalidOperationException(
                    "TCP non connesso.");

            await _stream.WriteAsync(data);
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;

            await _cts!.CancelAsync();

            _stream?.Dispose();
            _stream = null;

            _client?.Dispose();
            _client = null;

            _cts.Dispose();
            _cts = null;

            IsConnected = false;
        }
    }
}
