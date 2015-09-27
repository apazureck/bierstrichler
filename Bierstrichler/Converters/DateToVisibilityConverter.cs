using Bierstrichler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    public class DateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                throw new ArgumentException("Incoming value of \""+value.ToString()+"\" is of type \"" + value.GetType().ToString() + "\"", "value");
            if (parameter == null)
                parameter = new DateTime();
            if(!(parameter is DateTime))
                throw new ArgumentException("\"parameter\" is not a valid DateTime variable!", "parameter");

            return ((DateTime)value) != ((DateTime)parameter) ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
