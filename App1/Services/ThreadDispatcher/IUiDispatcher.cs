using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace SettingsClone.Services.ThreadDispatcher;


public interface IUiDispatcher
{
    bool TryEnqueue(DispatcherQueueHandler callback);
    void Initialize(DispatcherQueue dispatcherQueue);
}
