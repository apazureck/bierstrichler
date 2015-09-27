using Bierstrichler.Data;
using Bierstrichler.Functional;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    public class EnumToDisplayValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null  || !(value is Enum))
                return "";

            return EnumHelper.GetDescription(value as Enum);

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string) || !targetType.IsEnum)
                throw new ArgumentException("Wrong Types, Target has to be an Enum and the value has to be a string");
            Enum ret = EnumHelper.GetValueOfDescription((string)value, targetType);
            return ret;
        }
    }
}
