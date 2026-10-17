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

public class GeneralViewModel : ViewModelBase
{
    private readonly IConnectionService _connectionService;
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
    public GeneralViewModel()
    {
        _connectionService = App.Services.GetRequiredService<IConnectionService>();
    }

    /*....................................................................
    @
    @   Sorter IP address
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
    @   Sorter Subnet Mask 
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
    @   Sorter Gateway address
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
}