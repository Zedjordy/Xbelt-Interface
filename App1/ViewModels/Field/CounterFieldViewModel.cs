using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.ViewModels.Field;

public class CounterFieldViewModel : MessageFieldViewModel
{
    public int Current { get; set; }

    public string Start { get; set; }

    public string End { get; set; }


    public CounterFieldViewModel()
    {
        Type = TokenType.Counter;
    }
}
