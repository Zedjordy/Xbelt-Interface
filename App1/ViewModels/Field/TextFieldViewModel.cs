using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.ViewModels.Field;

public class TextFieldViewModel : MessageFieldViewModel
{
    public string Value { get; set; }

    public TextFieldViewModel()
    {
        Type = TokenType.Text;
    }
}
