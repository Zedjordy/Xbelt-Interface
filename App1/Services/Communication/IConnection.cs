using SettingsClone.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SettingsClone.Services.Communication
{
    /*....................................................................
     * 
     * IConnection manages and represent the single istance of a connection
     * 
     *....................................................................*/
    public interface IConnection
    {
        bool IsConnected { get; }
        ValueTask QueueSendAsync(
            byte[] message,
            CancellationToken cancellationToken = default);
        Task ConnectAsync();

        Task DisconnectAsync();

        Task SendAsync(CancellationToken token);        

        event EventHandler<byte[]>? DataReceived;
        event EventHandler<Exception>? Error;
    }
}
