using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly InfoBar _infoBar;

        public NotificationService(InfoBar infoBar)
        {
            _infoBar = infoBar;
        }

        public void Error(string message)
        {
            Show(message, InfoBarSeverity.Error);
        }

        public void Warning(string message)
        {
            Show(message, InfoBarSeverity.Warning);
        }

        public void Info(string message)
        {
            Show(message, InfoBarSeverity.Informational);
        }

        private void Show(string message, InfoBarSeverity severity)
        {
            _infoBar.Severity = severity;
            _infoBar.Message = message;
            _infoBar.IsOpen = true;
        }
    }
}
