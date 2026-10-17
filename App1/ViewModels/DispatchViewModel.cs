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

public class DispatchViewModel : ViewModelBase
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
    public DispatchViewModel()
    {
        _connectionService = App.Services.GetRequiredService<IConnectionService>();
    }
    /*....................................................................
    @
    @   Dispatch Server Port
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
    @   Dispatch Client Port
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
}