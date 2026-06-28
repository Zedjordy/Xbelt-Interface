using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using SettingsClone.Services;
using SettingsClone.ViewModels;
using SettingsClone.Views;
using System;

namespace SettingsClone;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    private Window? _window;

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        
        ConfigureServices(services);

        Services = services.BuildServiceProvider();

    }







    /*..........................................................*/
    #region METHODS
    /*..........................................................*/

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<FrameHolder>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<MainWindow>();

        //Pages
        services.AddTransient<ScannerPage>();
        services.AddTransient<MessageBuilderPage>();

        //ViewModel
        services.AddSingleton<ScannerPageViewModel>();
        services.AddTransient<MessageBuilderViewModel>();

        //services.AddTransient<MessageBuilderPage>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = Services.GetRequiredService<MainWindow>();
        _window.Activate();
    }

    #endregion
}