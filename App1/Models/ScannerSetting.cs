using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SettingsClone.Models;

public class ScannerSettings : ViewModelBase
{

    public Guid Id { get; } = Guid.NewGuid();



    private bool _IsEnabled = true;
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
    public ScannerSettings.ProtocolType? Protocol
    {
        get => _Protocol;
        set
        {
            _Protocol = (ScannerSettings.ProtocolType)value;
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
        get => Protocol == ScannerSettings.ProtocolType.UDP;
        set { if (value) Protocol = ScannerSettings.ProtocolType.UDP; }
    }

    public bool IsTcp
    {
        get => Protocol == ScannerSettings.ProtocolType.TCP;
        set { if (value) Protocol = ScannerSettings.ProtocolType.TCP; }
    }

    public ObservableCollection<MessageToken> InMessageTokens { get; } = new() { new MessageToken { Type = TokenType.STX, Name = "STX" } };
    public ObservableCollection<MessageToken> OutMessageTokens { get; } = new() { new MessageToken { Type = TokenType.STX, Name = "STX" } };

    public enum ProtocolType
    {
        UDP,
        TCP
    }
}