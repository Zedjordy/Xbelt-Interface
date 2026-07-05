using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace SettingsClone.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;
    private Frame? _frame;


    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate<TPage>(object? parameter = null)
        where TPage : Page
    {
        if (_frame is null)
            throw new InvalidOperationException("Frame non inizializzato");

        _frame.Navigate(typeof(TPage), parameter);

        Debug.WriteLine(_frame.CanGoBack);
    }

    public void GoBack()
    {
        if (_frame is null)
            throw new InvalidOperationException("Frame non inizializzato");

        if (_frame.CanGoBack)
            _frame.GoBack();
    }

    public bool CanGoBack => (_frame is not null)? _frame.CanGoBack : throw new InvalidOperationException("Frame non inizializzato");
}
