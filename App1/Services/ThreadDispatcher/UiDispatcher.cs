using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.ThreadDispatcher;

using Microsoft.UI.Dispatching;

public sealed class UiDispatcher : IUiDispatcher
{
    private DispatcherQueue _dispatcherQueue;

    public UiDispatcher()
    {
        //_dispatcherQueue = dispatcherQueue;
    }

    public bool TryEnqueue(DispatcherQueueHandler callback)
        => _dispatcherQueue.TryEnqueue(callback);

    public void Initialize(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }
}
