using SettingsClone.ViewModels.Scanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
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
        public event EventHandler<StringBuilder>? Notify_response;

        private readonly Channel<byte[]> _sendQueue =
            Channel.CreateBounded<byte[]>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

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

            Notify_response += _settings.Elaborate_response;
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

        public async Task SendAsync(CancellationToken token)
        {
            byte[] data = { 0, 0, 0 };
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

        /*....................................................................
    @
    @   QueueSendAsync
    @   ValueTask
    @
    @
    @
    @
    @
    @....................................................................*/
        public ValueTask QueueSendAsync(
            byte[] message,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            if (!IsConnected)
                throw new InvalidOperationException("Connection is not active.");

            return _sendQueue.Writer.WriteAsync(message, cancellationToken);
        }
    }
}
