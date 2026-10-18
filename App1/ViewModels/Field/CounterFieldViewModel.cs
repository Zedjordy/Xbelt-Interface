using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.ViewModels.Field;

public class CounterFieldViewModel : MessageFieldViewModel
{
    public int Current { get; set; }

    private string _Start;
    public string Start
    {
        get => _Start;

        set
        {
            SetProperty(ref _Start, value);
        }
    }

    private string _End;
    public string End
    {
        get => _End;

        set
        {
            SetProperty(ref _End, value);
        }
    }

    public CounterFieldViewModel()
    {
        Type = TokenType.Counter;
    }
}
