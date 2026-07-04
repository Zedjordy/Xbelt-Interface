using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone.Services;
using SettingsClone.Views;


namespace SettingsClone;

public sealed partial class MainWindow : Window
{

    private readonly INavigationService _navigation;

    public MainWindow(INavigationService navigation)
    {
        InitializeComponent();

        _navigation = navigation;

        _navigation.Initialize(ContentFrame);
        _navigation.Navigate<ScannerPage>();


    }





    private void NavView_SelectionChanged(NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer is NavigationViewItem item)
        {
            switch (item.Tag?.ToString())
            {
                case "Scanner":
                    _navigation.Navigate<ScannerPage>();
                    break;
            }
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (_navigation.CanGoBack)
            _navigation.GoBack();
    }
}