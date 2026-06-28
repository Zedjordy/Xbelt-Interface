using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

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

        // 1. crea la Page via DI
        var page = _provider.GetRequiredService<TPage>();

        // 2. set frame content
        _frame.Content = page;

        // 3. dispatch parameter manuale
        if (page is INavigationAware aware)
        {
            aware.OnNavigatedTo(parameter);
        }
    }
}
