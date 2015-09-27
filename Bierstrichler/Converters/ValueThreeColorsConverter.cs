using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Bierstrichler.Converters
{
    public class ValueThreeColorsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.LightGreen);

            Double doubleValue = 0.0;
            Double.TryParse(value.ToString(), out doubleValue);

            if (Math.Abs(doubleValue) < 2 * double.Epsilon)
                brush = new SolidColorBrush(Colors.White);
            else if (doubleValue < 0)
                brush = new SolidColorBrush(Colors.Tomato);

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
