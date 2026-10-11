using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services;

internal class BaseLib
{
}

public static class Log
{
    public static void Write(string message)
    {
        Debug.WriteLine(
            $"[{DateTimeOffset.Now:HH:mm:ss.fff}] {message}");
    }
}
