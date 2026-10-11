using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    /*....................................................................
     * 
     * IConnectionService manages and organize all the opened connections
     * 
     *....................................................................*/
    public interface IConnectionService
    {
        Task OpenConnectionAsync(ScannerViewModel scanner);

        Task CloseConnectionAsync(ScannerViewModel scanner);

        Task SendAsync(ScannerViewModel scanner, byte[] data);
    }
}
