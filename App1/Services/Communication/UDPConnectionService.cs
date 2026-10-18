using SettingsClone.Services.Monitoring;
using SettingsClone.ViewModels;
using SettingsClone.ViewModels.Scanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using static SettingsClone.Models.MonitoredMessage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SettingsClone.Services.Communication
{
    public class UDPConnectionService : IConnection
    {
        public readonly ScannerViewModel _scanner;
        public readonly GeneralViewModel _sorter;
        private readonly IMessageMonitorService _monitor;


        private UdpClient? _udpClient;
        private CancellationTokenSource? _cts;
        public bool IsConnected { get; private set; }
        private Task? _receiveTask;
        private Task? _sendTask;

        public event EventHandler<byte[]>? DataReceived;
        public event EventHandler<Exception>? Error;
        public event EventHandler<string>? MessageReceived;

        private readonly Channel<byte[]> _sendQueue =
            Channel.CreateBounded<byte[]>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
        /*....................................................................
        @
        @   UDPConnectionService
        @   CONSTRUCTOR
        @
        @
        @
        @
        @
        @....................................................................*/
        public UDPConnectionService(IMessageMonitorService monitor, ScannerViewModel settings, GeneralViewModel endpoint_settings)
        {
            _scanner = settings;
            _sorter = endpoint_settings;
            _monitor = monitor;

        }

        /*....................................................................
        @
        @   ConnectAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;


            IPAddress ip;
            _udpClient = new UdpClient();


            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                Debug.WriteLine(
                    $"NIC: {ni.Name} - {ni.OperationalStatus} - {ni.NetworkInterfaceType}");

                foreach (var address in ni.GetIPProperties().UnicastAddresses)
                {
                    Debug.WriteLine(
                        $"    IP: {address.Address} " +
                        $"Family: {address.Address.AddressFamily}");
                }
            }


            if (!IPAddress.TryParse(string.Join('.', _scanner.IpAddress), out ip))
                throw new FormatException("Invalid IP address");

            //Wait for network interface is up and connected                 
            await WaitForInterfaceUpAsync(
                    _scanner.Net_Interface,
                    _cts?.Token ?? CancellationToken.None);

            //Bind the connection between Sorter IP and Scanner IP
            _udpClient.Client.Bind(new IPEndPoint(ip, _scanner.ServerPort));

            //_udpClient.Connect(ip, _scanner.ClientPort);

            _cts = new CancellationTokenSource();
            IsConnected = true;


            _receiveTask = RecvAsync(_cts.Token);
            _sendTask = SendAsync(_cts.Token);

            //await Task.CompletedTask;
        }


        /*....................................................................
        @
        @   DisconnectAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;

            await _cts!.CancelAsync();

            _udpClient?.Dispose();
            _udpClient = null;

            _cts.Dispose();
            _cts = null;

            IsConnected = false;
        }


        /*....................................................................
        @
        @   RecvAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        public async Task RecvAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _udpClient!.ReceiveAsync(cancellationToken);

                    _monitor.TryPublish(
                        _scanner.Id.ToString(),
                        _scanner.Name,
                        MessageDirection.Rx,
                        result.RemoteEndPoint.ToString(),
                        result.Buffer);

                    DataReceived?.Invoke(this, result.Buffer);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _udpClient.Close();
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, ex);
            }



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
        public async Task SendAsync(CancellationToken cancellationToken)
        {
            var endpoint_ip = IPAddress.Parse(_sorter.IpAddress);

            if (_udpClient == null)
                throw new InvalidOperationException("UDP client not initialized");
            try
            {
                await foreach (var message in
                    _sendQueue.Reader.ReadAllAsync(cancellationToken))
                {
                    await _udpClient.SendAsync(message,
                        new IPEndPoint(endpoint_ip,
                        _scanner.ClientPort), cancellationToken);


                    /*Publish the message into the monitoring service*/
                    _monitor.TryPublish(
                        _scanner.Id.ToString(),
                        _scanner.Name,
                        MessageDirection.Tx,
                        endpoint_ip.ToString(),
                        message);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Arresto previsto.
            }
        }


        /*....................................................................
        @
        @   WaitForInterfaceUpAsync
        @   Task
        @
        @
        @
        @
        @
        @....................................................................*/
        private static async Task WaitForInterfaceUpAsync(
        NetworkInterface networkInterface,
        CancellationToken cancellationToken)
        {
            while (networkInterface.OperationalStatus != OperationalStatus.Up)
            {
                await Task.Delay(500, cancellationToken);
            }
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
