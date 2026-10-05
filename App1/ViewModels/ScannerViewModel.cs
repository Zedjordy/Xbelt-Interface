using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace SettingsClone.ViewModels;

public class ScannerViewModel : ViewModelBase
{



    public Guid Id { get; } = Guid.NewGuid();

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


    private bool _IsEnabled = false;
    public bool IsEnabled
    {
        get => _IsEnabled;
        set
        {
            _IsEnabled = value;
            OnPropertyChanged();
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

    private NetworkInterface _Net_Interface;
    public NetworkInterface Net_Interface
    {
        get => _Net_Interface;
        set
        {
            _Net_Interface = value;
            OnPropertyChanged();
        }
    }

    private IPAddress _selected_Ip;
    public IPAddress selected_Ip
    {
        get => _selected_Ip;
        set
        {
            _selected_Ip = value is not null ? IPAddress.Parse(value.ToString()) : IPAddress.Parse("0.0.0.0");
            IpAddress = _selected_Ip.ToString();
            OnPropertyChanged();
        }
    }


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

    public bool IsUdp
    {
        get => Protocol == ScannerViewModel.ProtocolType.UDP;
        set { if (value) Protocol = ScannerViewModel.ProtocolType.UDP; }
    }

    public bool IsTcp
    {
        get => Protocol == ScannerViewModel.ProtocolType.TCP;
        set { if (value) Protocol = ScannerViewModel.ProtocolType.TCP; }
    }

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

    public ObservableCollection<MessageTokenViewModel> InMessageTokens { get; } = new() { new MessageTokenViewModel { Type = TokenType.STX, Name = "STX", Value = "\x02", Length = 1 } };
    public ObservableCollection<MessageTokenViewModel> OutMessageTokens { get; } = new() { new MessageTokenViewModel { Type = TokenType.STX, Name = "STX", Value = "\x02", Length = 1 } };



    public enum ProtocolType
    {
        UDP,
        TCP
    }
}