using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SettingsClone.ViewModels;
using System;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace SettingsClone.Converters;

public class ProtocolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ScannerViewModel.ProtocolType protocol && parameter is ScannerViewModel.ProtocolType target)
        {
            return protocol == target;
        }

        return false;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isChecked && isChecked && parameter is string target)
        {
            return target;
        }

        return null;
    }
}