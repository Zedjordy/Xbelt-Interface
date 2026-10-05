using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    public interface IConnection
    {
        Task ConnectAsync();

        Task DisconnectAsync();

        Task SendAsync(byte[] data);

        //Task RecvAsync(CancellationToken cancellationToken);

        event EventHandler<byte[]>? DataReceived;
        event EventHandler<Exception>? Error;
    }
}
