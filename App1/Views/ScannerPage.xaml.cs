using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SettingsClone.Views;

public sealed partial class ScannerPage : Page
{
    public ScannerPageViewModel ViewModel { get; }


    public ScannerPage()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<ScannerPageViewModel>();
        DataContext = ViewModel;
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

}