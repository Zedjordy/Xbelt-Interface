using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SettingsClone.ViewModels.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsClone.Converters
{
    public class MessageFieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextFieldTemplate { get; set; }
        public DataTemplate RangeFieldTemplate { get; set; }
        public DataTemplate CounterFieldTemplate { get; set; }
        public DataTemplate ChoiceFieldTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                TextFieldViewModel => TextFieldTemplate,
                RangeFieldViewModel => RangeFieldTemplate,
                CounterFieldViewModel => CounterFieldTemplate,
                ChoiceFieldViewModel => ChoiceFieldTemplate,
                _ => null
            };
        }
    }
}
