using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using SettingsClone.Services;
using SettingsClone.Services.Communication;
using SettingsClone.Services.Field;
using SettingsClone.Services.Monitoring;
using SettingsClone.Services.Notification;
using SettingsClone.Services.ThreadDispatcher;
using SettingsClone.ViewModels;
using SettingsClone.ViewModels.MessageBuilder;
using SettingsClone.ViewModels.Monitor;
using SettingsClone.ViewModels.Scanner;
using SettingsClone.Views;
using System;
using System.Runtime.CompilerServices;

namespace SettingsClone;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    private Window? _window;
    IHost host;

    public App()
    {
        InitializeComponent();

        var services = new ServiceCollection();

        host = Host.CreateDefaultBuilder()

        .ConfigureServices(services =>
        {
            ConfigureServices(services);
        })
        .Build();

        //Services = services.BuildServiceProvider();
        Services = host.Services;
    }







    /*..........................................................*/
    #region METHODS
    /*..........................................................*/
    private static void ConfigureServices(IServiceCollection services)
    {
        //Message Monitor
        services.AddSingleton(new MessageMonitorOptions
        {
            HistoryCapacity = 10_000,
            ChannelCapacity = 4_096,
            MaxVisibleMessages = 2_000,
            RefreshIntervalMs = 100
        });

        services.AddSingleton<MessageMonitorService>();

        services.AddSingleton<IMessageMonitorService>(
            sp => sp.GetRequiredService<MessageMonitorService>());

        services.AddHostedService(
            sp => sp.GetRequiredService<MessageMonitorService>());

        services.AddTransient<MessageMonitorViewModel>();
        services.AddTransient<MessageMonitorPage>();
        //...........................................................................

        //Applicative Services
        services.AddSingleton<FrameHolder>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IMessageToken, FieldsService>();
        services.AddSingleton<IConnectionService, ScannerConnectionService>();
        services.AddSingleton<UiDispatcher>();
        services.AddSingleton<IUiDispatcher>(
            sp => sp.GetRequiredService<UiDispatcher>());
        //services.AddSingleton<INotificationService, NotificationService>();
        //...........................................................................

        //
        services.AddSingleton<IConnection, UDPConnectionService>();
        services.AddSingleton<IConnection, TCPConnectionService>();
        //...........................................................................

        //Window
        services.AddSingleton<MainWindow>();
        //...........................................................................

        /*-----------------
         * Pages
         *-----------------*/
        //  ->  Scanner
        services.AddTransient<ScannerPage>();
        services.AddTransient<MessageBuilderPage>();

        //  ->  Message Monitor
        services.AddTransient<MessageMonitorPage>();

        //  ->  General
        services.AddTransient<GeneralPage>();

        //  ->  Dispatch
        services.AddTransient<DispatchPage>();
        //...........................................................................

        /*-----------------
         * VieModels
         *-----------------*/
        //  ->  Scanner
        services.AddSingleton<ScannerPageViewModel>();
        services.AddTransient<MessageBuilderViewModel>();
        
        //  ->  Message Monitor
        services.AddSingleton<MessageMonitorViewModel>();
        
        //  ->  General
        services.AddSingleton<GeneralViewModel>();
        
        //  ->  Network
        services.AddTransient<NetworkViewModel>();

        //  ->  Dispatch
        services.AddSingleton<DispatchViewModel>();
        //...........................................................................
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        await host.StartAsync();

        _window = Services.GetRequiredService<MainWindow>();

        Services.GetRequiredService<IUiDispatcher>()
            .Initialize(_window.DispatcherQueue);

        _window.Activate();

    }

    #endregion
}