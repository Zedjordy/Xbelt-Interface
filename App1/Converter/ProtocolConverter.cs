using Microsoft.UI.Xaml.Data;
using SettingsClone.Models;
using System;
using System.Net.Sockets;

namespace SettingsClone.Converters;

public class ProtocolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ScannerSettings.ProtocolType protocol && parameter is ScannerSettings.ProtocolType target)
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