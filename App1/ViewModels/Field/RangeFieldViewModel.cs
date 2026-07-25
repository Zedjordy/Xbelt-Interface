using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.ViewModels.Field;

public class RangeFieldViewModel : MessageFieldViewModel
{
    public enum RangeType
    {
        Numeric,
        Alphabetic,
        AlphaNumeric
    }

    public IEnumerable<RangeType> Formats =>
    Enum.GetValues<RangeType>();

    public RangeType Format { get; set; }

    public string Start { get; set; }

    public string End { get; set; }

    public int Length { get; set; }


    public RangeFieldViewModel()
    {
        Type = TokenType.Range;
    }
}
