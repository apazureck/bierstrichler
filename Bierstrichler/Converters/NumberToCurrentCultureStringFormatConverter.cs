using Bierstrichler.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    public class NumberToCurrentCultureStringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(parameter is string))
                throw new ArgumentException("Format string is no string.", "parameter");
            culture = CultureInfo.CurrentCulture;
            
            if (value.GetType() == typeof(decimal))
                return ((decimal)value).ToString((string)parameter, culture);
            if (value.GetType() == typeof(int))
                return ((int)value).ToString((string)parameter, culture);
            if (value.GetType() == typeof(double))
                return ((double)value).ToString((string)parameter, culture);
            throw new ArgumentException(value.GetType().ToString() + " is not a valid number value type.", "value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            culture = CultureInfo.CurrentCulture;

            return System.Convert.ToString(value, culture);
        }
    }
}
