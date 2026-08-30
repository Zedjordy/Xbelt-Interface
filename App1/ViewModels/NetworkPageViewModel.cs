using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone;
using SettingsClone.Services;
using SettingsClone.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SettingsClone.ViewModels
{

    public class NetworkPageViewModel : ViewModelBase
    {
        private readonly INavigationService _nav;
        public ObservableCollection<ScannerViewModel> Scanners { get; set; }


        #region CONSTRUCTOR

        public NetworkPageViewModel(INavigationService nav)
        {
            _nav = nav;

        }
        #endregion


        public void Initialize(object parameter)
        {
            if (parameter is ScannerPageViewModel scanner)
            {
                Scanners = scanner.Scanners;
            }

        }

    }
}
