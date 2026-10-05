using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    public interface IConnectionService
    {
        Task ConnectAsync(ScannerViewModel scanner);

        Task DisconnectAsync(ScannerViewModel scanner);

        Task SendAsync(
            ScannerViewModel scanner,
            byte[] data);
    }
}
