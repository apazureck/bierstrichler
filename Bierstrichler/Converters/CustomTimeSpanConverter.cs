using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(String))]
    public class CustomTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan ts = (TimeSpan)value;
            return ts.ToString((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string tsString = value as string;
            if (tsString == null)
                throw new ArgumentException("Argument is not a string.");

            Match m = Regex.Match(tsString, @"(?<hours>\d+).*h.*(?<minutes>\d{1,2}).*m");
            if (m == null)
                throw new ArgumentException("Not a valid String.");
            return TimeSpan.Parse(m.Groups["hours"].Value + ":" + m.Groups["minutes"].Value);
        }
    }
}
