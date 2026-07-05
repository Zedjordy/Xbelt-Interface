using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SettingsClone.Models;
using System;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace SettingsClone.Converters;

public class MessageTokenColorConvert : IValueConverter
{


    public object Convert(object value, Type targetType, object parameter, string language)
    {
        Brush fallback = (Brush)Microsoft.UI.Xaml.Application.Current.Resources["AccentFillColorSecondaryBrush"];

        if (value is not TokenType token)
            return fallback;

        return token switch
        {
            TokenType.Barcode => GetSolidColor(Colors.SteelBlue),
            TokenType.Constant => GetSolidColor(Colors.LightSlateGray),
            TokenType.Date => GetSolidColor(Colors.Gray),
            TokenType.STX => GetSolidColor(Colors.Transparent),
            TokenType.ETX => GetSolidColor(Colors.Transparent),
            TokenType.Index => GetSolidColor(Colors.DodgerBlue),
            TokenType.Separator => GetSolidColor(Colors.LightSlateGray),
            TokenType.Time => GetSolidColor(Colors.Gray),

                _ => fallback
    };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isChecked && isChecked && parameter is string target)
        {
            return target;
        }

        return null;
    }

    private SolidColorBrush GetSolidColor(Windows.UI.Color key)
    {
        return new SolidColorBrush(key);
    }
}