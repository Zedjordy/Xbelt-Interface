using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Notification
{
    public interface INotificationService
    {
        void Error(string message);
        void Warning(string message);
        void Info(string message);
    }
}
