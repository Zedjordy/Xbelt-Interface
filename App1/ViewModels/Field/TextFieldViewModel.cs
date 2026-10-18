using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.ViewModels.Field;

public class TextFieldViewModel : MessageFieldViewModel
{    
    private string _value = "";
    public string Value
    {
        get => _value;
        
        set
        {
            SetProperty(ref _value, value);
        }
    }

    public TextFieldViewModel()
    {
        Type = TokenType.Text;
    }
}
