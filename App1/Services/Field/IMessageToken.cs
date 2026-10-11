using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Services.Field
{
    public interface IMessageToken
    {
        public StringBuilder Elaborate_field(MessageFieldViewModel field);
    }
}
