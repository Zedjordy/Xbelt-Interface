using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using SettingsClone.Services;
using SettingsClone.Services.Communication;
using SettingsClone.Services.Field;
using SettingsClone.Services.Notification;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SettingsClone.ViewModels.Field.RangeFieldViewModel;

namespace SettingsClone.ViewModels;

public class ScannerViewModel : ViewModelBase
{
    public event EventHandler<StringBuilder>? Notify_response;
    private readonly IConnectionService _connectionService;
    private readonly IMessageToken _messagetokenService;
    private List<ResponseOutcome> Options = new(Enum.GetValues<ResponseOutcome>());
    /*....................................................................
    @
    @   SCANNER VIEW MODEL
    @   CONSTRUCTOR
    @
    @
    @
    @
    @
    @....................................................................*/
    public ScannerViewModel()
    {
        _connectionService = App.Services.GetRequiredService<IConnectionService>();
        _messagetokenService = App.Services.GetRequiredService<IMessageToken>();
    }


    /*....................................................................
    @
    @   Scanner ID (Global Unique IDentifier)
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    public Guid Id { get; } = Guid.NewGuid();

    /*....................................................................
    @
    @   Scanner NAME
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private string _Name = "Scanner";
    public string Name
    {
        get => _Name;
        set
        {
            _Name = value;
            OnPropertyChanged();
        }
    }

    /*....................................................................
    @
    @   Status Connected 
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private bool _IsConnected = false;
    public bool IsConnected
    {
        get => _IsConnected;
        set
        {
            _IsConnected = value;
            OnPropertyChanged();

            //Logic
            Check_Connection();
        }
    }

    private ProtocolType _Protocol = ProtocolType.UDP;
    public ScannerViewModel.ProtocolType? Protocol
    {
        get => _Protocol;
        set
        {
            _Protocol = (ScannerViewModel.ProtocolType)value;
            OnPropertyChanged();
        }
    }


    /*....................................................................
    @
    @   Network Interface
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private NetworkInterface _Net_Interface;
    public NetworkInterface Net_Interface
    {
        get => _Net_Interface;
        set
        {
            _Net_Interface = value;
            OnPropertyChanged();


            /*  Logic of the Property*/

            ObservableCollection<IPAddress> ipProps = new();
            var _ipProps = _Net_Interface.GetIPProperties();

            ipProps.Clear();

            // Get IPv4 addresses (skip IPv6)
            foreach (var ipAddr in _ipProps.UnicastAddresses)
            {
                if (ipAddr.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4
                {
                    ipProps.Add(ipAddr.Address);
                    Console.WriteLine($"Interface: {_Net_Interface.Name}, IP: {ipAddr.Address}");
                }
            }

            this.IPlist = ipProps;
            this.SubnetMask = "0.0.0.0";
            this.Gateway = "0.0.0.0";
        }
    }
    /*....................................................................
    @
    @   Selected IP
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private IPAddress _selected_Ip;
    public IPAddress selected_Ip
    {
        get => _selected_Ip;
        set
        {
            _selected_Ip = value is not null ? IPAddress.Parse(value.ToString()) : IPAddress.Parse("0.0.0.0");
            IpAddress = _selected_Ip.ToString();
            OnPropertyChanged();


            /*Logic of the property*/

            var nic = this.Net_Interface;

            if (nic is not null && value is not null)
            {
                var ipInfo = nic.GetIPProperties()
                    .UnicastAddresses
                    .First(a => a.Address.Equals(this.selected_Ip));

                this.SubnetMask = ipInfo.IPv4Mask.ToString();

                var gateway = nic.GetIPProperties()
                    .GatewayAddresses
                    .FirstOrDefault(g =>
                        g.Address.AddressFamily == AddressFamily.InterNetwork)
                    ?.Address;


                this.Gateway = gateway?.ToString() ?? "0.0.0.0";
            }
        }
    }

    /*....................................................................
    @
    @   IP address list retrieved from the selected Network Interface
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private ObservableCollection<IPAddress> _IPlist;
    public ObservableCollection<IPAddress> IPlist
    {
        get => _IPlist;
        set
        {
            _IPlist = value;
            OnPropertyChanged();
        }
    }

    /*....................................................................
    @
    @   Scanner IP address
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private string _IpAddress;
    public string IpAddress
    {
        get => _IpAddress;
        set
        {
            _IpAddress = value;
            OnPropertyChanged();
        }
    }
    /*....................................................................
    @
    @   Scanner Subnet Mask 
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private string _SubnetMask;
    public string SubnetMask
    {
        get => _SubnetMask;
        set
        {
            _SubnetMask = value;
            OnPropertyChanged();
        }
    }
    /*....................................................................
    @
    @   Scanner Gateway address
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private string _Gateway;
    public string Gateway
    {
        get => _Gateway;
        set
        {
            _Gateway = value;
            OnPropertyChanged();
        }
    }

    /*....................................................................
    @
    @   Scanner Server Port
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private int _ServerPort = 0;
    public int ServerPort
    {
        get => _ServerPort;
        set
        {
            if (value < 0 || value > 65535) return;
            _ServerPort = value;
            OnPropertyChanged();
        }
    }
    /*....................................................................
    @
    @   Scanner Client Port
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private int _ClientPort = 0;
    public int ClientPort
    {
        get => _ClientPort;
        set
        {
            if (value < 0 || value > 65535) return;
            _ClientPort = value;
            OnPropertyChanged();
        }
    }
    /*....................................................................
    @
    @   Flag UDP connection
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    public bool IsUdp
    {
        get => Protocol == ScannerViewModel.ProtocolType.UDP;
        set { if (value) Protocol = ScannerViewModel.ProtocolType.UDP; }
    }
    /*....................................................................
    @
    @   Flag TCP connection
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    public bool IsTcp
    {
        get => Protocol == ScannerViewModel.ProtocolType.TCP;
        set { if (value) Protocol = ScannerViewModel.ProtocolType.TCP; }
    }
    /*....................................................................
    @
    @   Response TX delay
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private int _TxDelay;
    public int TxDelay
    {
        get => _TxDelay;
        set
        {
            if (value < 0 || value > 65535) return;
            _TxDelay = value;
            OnPropertyChanged();
        }
    }
    /*....................................................................
    @
    @   NO-READ response percentage
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private uint _NOREAD_perc;
    public uint NOREAD_perc
    {
        get => _NOREAD_perc;
        set
        {
            int delta = (int)value - (int)_NOREAD_perc;

            if (delta > 0 && ((GOODREAD_perc - delta) < 0))
            {
                MULTIREAD_perc = (uint)Math.Max(0, MULTIREAD_perc - delta);
            }

            if (SetProperty(ref _NOREAD_perc, value))
            {
                OnPropertyChanged(nameof(GOODREAD_perc));
            }
        }
    }
    /*....................................................................
    @
    @   MULTIREAD response percentage
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private uint _MULTIREAD_perc;
    public uint MULTIREAD_perc
    {
        get => _MULTIREAD_perc;
        set
        {
            int delta = (int)value - (int)_MULTIREAD_perc;

            if (delta > 0 && ((GOODREAD_perc - delta) < 0))
            {
                NOREAD_perc = (uint)Math.Max(0, NOREAD_perc - delta);
            }
            if (SetProperty(ref _MULTIREAD_perc, value))
            {
                OnPropertyChanged(nameof(GOODREAD_perc));
            }
        }
    }
    /*....................................................................
    @
    @   GOOD READ response percentage
    @   Property
    @
    @
    @
    @
    @
    @....................................................................*/
    private uint _GOODREAD_perc = 100;
    public uint GOODREAD_perc => 100 - NOREAD_perc - MULTIREAD_perc;



    private char _NOREAD_char = '?';
    public char NOREAD_char
    {
        get => _NOREAD_char;
        set
        {
            _NOREAD_char = value;
            OnPropertyChanged();
        }
    }

    private char _MULTIREAD_char = '9';
    public char MULTIREAD_char
    {
        get => _MULTIREAD_char;
        set
        {
            _MULTIREAD_char = value;
            OnPropertyChanged();
        }
    }

    S_wk_scanner s_wk_scanner;

    private struct S_wk_scanner
    {
        public S_msg_sent msg_sent;
    }
    private struct S_msg_sent
    {
        public uint total_msg;
        public uint good_read;
        public uint noread_read;
        public uint multi_read;

    }
    public enum ResponseOutcome
    {
        GOODREAD,
        NOREAD,
        MULTIREAD
    }
    public ObservableCollection<MessageTokenViewModel> InMessageTokens { get; } = new() { new MessageTokenViewModel { Type = TokenType.STX, Name = "STX", Value = "\x02", Length = 1 } };
    public ObservableCollection<MessageTokenViewModel> OutMessageTokens { get; } = new() { new MessageTokenViewModel { Type = TokenType.STX, Name = "STX", Value = "\x02", Length = 1 } };


    /*....................................................................
    @
    @   Protocol connection type
    @   Enumeration
    @
    @
    @
    @
    @
    @....................................................................*/
    public enum ProtocolType
    {
        UDP,
        TCP
    }

    public async void Check_Connection()
    {
        if (_IsConnected && this.selected_Ip is not null)
        {
            await _connectionService.OpenConnectionAsync(this);
        }
        else
        {
            await _connectionService.CloseConnectionAsync(this);
        }
    }

    /*.................................................................... 
    *   Connection_DataReceived
    *   public method
    *
    *
    *
    *
    *
    *....................................................................*/
    public void OnDataReceived(object? sender, byte[] data)
    {
        var _connection = (UDPConnectionService)sender;
        var message_setting = (ScannerViewModel)_connection._scanner;

        StringBuilder RX_message = new();
        var token_list = message_setting.InMessageTokens;
        string start, end;
        uint configured_rx_length = 0;
        uint index_char = 0;
        start = end = "";

        //compute the length of the expected string
        foreach (var token in token_list)
        {
            configured_rx_length += token.Length;
        }

        //if the string length received differs from the expected
        if (configured_rx_length != data.Length)
        {
            // pacchetti dagli scanner ricevuti
            Log.Write($"SCANNER {message_setting.Name} <OnDataReceived> **IGNORE MESSAGE** Length of the message received is {data.Length} bytes, different than the expected {configured_rx_length} bytes");
            return;
        }

        /*.........................................................
         * 
         * cycle among the configured expected token of the message
         * -if the token configured has length 1 and has an expected
         * value check if the received char matches the value.
         * -if the expected token has only a configured length
         * continue the iteration among the next chars and append 
         * them to the complete string.
         * 
         *..........................................................*/
        foreach (var token in token_list)
        {
            if (token.Length == 1 && (byte)token.Value[0] != 0)
            {
                if (data[index_char] != (byte)token.Value[0])
                {
                    Log.Write($"SCANNER {message_setting.Name} <OnDataReceived> token received '{data[index_char]}' different than the expected '{token.Value}'");
                }
                else
                {
                    //RX_message.Append((char)data[index_char]);
                    if (data[index_char] == 2)
                    {
                        start = "<STX>";
                        Log.Write($"SCANNER {message_setting.Name} <OnDataReceived> STX token found");
                    }
                    else if (data[index_char] == 3)
                    {
                        end = "<ETX>";
                        Log.Write($"SCANNER {message_setting.Name} <OnDataReceived> ETX token found");
                    }
                }
                index_char++;
            }
            else
            {
                var tempValue = new StringBuilder();

                for (int x = 0; x < token.Length; x++, index_char++)
                {
                    tempValue.Append((char)data[index_char]);
                    RX_message.Append((char)data[index_char]);
                }
                token.Value = tempValue.ToString();
            }
        }

        Log.Write($"SCANNER {message_setting.Name} <OnDataReceived> Received string {start}{RX_message}{end}");


        /*........................................
         * Trigger the message response elaboration
         *........................................*/
        Notify_response?.Invoke(sender, RX_message);
    }

    /*....................................................................
     * 
     *  Elaborate_response
     *  private method
     * 
     * 
     * 
     * 
     * 
     * 
     * ...................................................................*/
    async public void Elaborate_response(object? sender, StringBuilder data)
    {
        UDPConnectionService _connection = (UDPConnectionService)sender;
        var message_setting = (ScannerViewModel)_connection._scanner;

        StringBuilder TX_message = new();
        ResponseOutcome outcome;

        outcome = OutcomeBalancer();

        foreach (MessageTokenViewModel token in message_setting.OutMessageTokens)
        {
            /*..............................
             * If the token has a configured value (or it's a token referenced from the Inbound)
             *..............................*/
            if (token.Value != "")
            {
                TX_message.Append(token.Value);
            }
            /*..............................
             * Or if the token is without value means that has to be built by the fields its configured with
             *..............................*/
            else if (token.Fields.Count > 0)
            {

                if (token.Type == TokenType.Barcode)
                {
                    switch (outcome)
                    {
                        case (ResponseOutcome.NOREAD):
                            TX_message.Append(this.NOREAD_char, (int)token.Length);
                            break;

                        case (ResponseOutcome.MULTIREAD):
                            TX_message.Append(this.MULTIREAD_char, (int)token.Length);
                            break;

                        case (ResponseOutcome.GOODREAD):
                        default:
                            foreach (MessageFieldViewModel field in token.Fields)
                            {
                                TX_message.Append(_messagetokenService.Elaborate_field(field));
                            }
                            break;

                    }
                }
                else
                {
                    foreach (MessageFieldViewModel field in token.Fields)
                    {
                        TX_message.Append(_messagetokenService.Elaborate_field(field));
                    }
                }

            }
        }

        //Wait time delay for sending response
        await Task.Delay(message_setting.TxDelay);

        //Add message in the QueueSend of the driver
        await _connection.QueueSendAsync(Encoding.ASCII.GetBytes(TX_message.ToString()));

        Log.Write($"SCANNER {message_setting.Name} <Elaborate_response> TX response '{TX_message}'");
    }

    private ResponseOutcome OutcomeBalancer()
    {
        ResponseOutcome outcome;
        uint from;
        uint to;

        Random random = new();

        if (this.s_wk_scanner.msg_sent.total_msg > 99)
        {
            this.s_wk_scanner.msg_sent.total_msg = 0;
            this.s_wk_scanner.msg_sent.good_read = 0;
            this.s_wk_scanner.msg_sent.noread_read = 0;
            this.s_wk_scanner.msg_sent.multi_read = 0;
            Options.Clear();
            Options.AddRange(Enum.GetValues<ResponseOutcome>());
        }
        else
        {

            if (this.s_wk_scanner.msg_sent.good_read >= this.GOODREAD_perc)
            {
                Options.Remove(ResponseOutcome.GOODREAD);
            }
            if (this.s_wk_scanner.msg_sent.noread_read >= this.NOREAD_perc)
            {
                Options.Remove(ResponseOutcome.NOREAD);
            }
            if (this.s_wk_scanner.msg_sent.multi_read >= this.MULTIREAD_perc)
            {
                Options.Remove(ResponseOutcome.MULTIREAD);
            }

        }

        switch (Options[random.Next(Options.Count)])
        {


            case (ResponseOutcome.NOREAD):
                outcome = ResponseOutcome.NOREAD;
                this.s_wk_scanner.msg_sent.noread_read++;
                break;

            case (ResponseOutcome.MULTIREAD):
                outcome = ResponseOutcome.MULTIREAD;
                this.s_wk_scanner.msg_sent.multi_read++;
                break;

            case (ResponseOutcome.GOODREAD):
            default:
                outcome = ResponseOutcome.GOODREAD;
                this.s_wk_scanner.msg_sent.good_read++;
                break;

        }

        this.s_wk_scanner.msg_sent.total_msg++;

        Log.Write($"SCANNER {this.Name} <OutcomeBalancer> GOODREAD: {this.s_wk_scanner.msg_sent.good_read}");
        Log.Write($"SCANNER {this.Name} <OutcomeBalancer> NOREAD: {this.s_wk_scanner.msg_sent.noread_read}");
        Log.Write($"SCANNER {this.Name} <OutcomeBalancer> MULTIREAD: {this.s_wk_scanner.msg_sent.multi_read}");
        Log.Write($"SCANNER {this.Name} <OutcomeBalancer> TOTAL: {this.s_wk_scanner.msg_sent.total_msg}");



        return outcome;
    }
}