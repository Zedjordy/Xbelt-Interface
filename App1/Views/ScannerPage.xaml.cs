using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Services;
using SettingsClone.Services.Communication;
using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class ScannerPage : Page
{
    public ScannerPageViewModel ViewModel { get; }
    public event EventHandler<ToggleSwitch> ConnectionChanged;
    public event EventHandler<ComboBox> GetIps;
    public event EventHandler<ComboBox> GetIpInfo;

    public ScannerPage()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<ScannerPageViewModel>();
        DataContext = ViewModel;

        ConnectionChanged += (s, toggle) => ViewModel.Check_Connection(toggle);
        GetIps += (s, cbox) => ViewModel.GetIps(cbox);
        GetIpInfo += (s, cbox) => ViewModel.GetIpInfo(cbox);
    }


protected override void OnNavigatedTo(NavigationEventArgs e)
{
    base.OnNavigatedTo(e);

    var param = (string)e.Parameter;
}

    private void Slider_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var slider = (Slider)sender;

        int delta = e.GetCurrentPoint(slider).Properties.MouseWheelDelta;

        if (delta > 0)
        {
            slider.Value = Math.Min(slider.Maximum, slider.Value + slider.StepFrequency);
        }
        else if (delta < 0)
        {
            slider.Value = Math.Max(slider.Minimum, slider.Value - slider.StepFrequency);
        }

        e.Handled = true;
    }

    private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch toggle)
            return;

        ConnectionChanged?.Invoke(this, toggle);
    }

    private void Combobox_GetIps(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox cbox)
            return;

        GetIps?.Invoke(this, cbox);
    }
    private void Combobox_GetIpInfo(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox cbox)
            return;

        GetIpInfo?.Invoke(this, cbox);
    }


}