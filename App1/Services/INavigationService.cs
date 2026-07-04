using Microsoft.UI.Xaml.Controls;
using System;

namespace SettingsClone.Services;

public interface INavigationService
{
    void Initialize(Frame frame);

    void Navigate<TPage>(object? parameter = null)
        where TPage : Page;

    void GoBack();
    bool CanGoBack { get; }
}