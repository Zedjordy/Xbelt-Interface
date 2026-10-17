using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Services;
using SettingsClone.Services.Notification;
using SettingsClone.Services.Communication;
using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class DispatchPage : Page
{
    public DispatchPage()
    {
        InitializeComponent();
    }
}
