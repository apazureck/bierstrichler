using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    /// <summary>
    /// Convertiert eine Datetime und ordnet sie zum sortieren dem Vortag zu, sofern das Getränk vor 8.00 Uhr gekauft wurde.
    /// </summary>
    public class DateTimeToEveningConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime? date = value as DateTime?;
            if(date == null)
                return null;
            if (date.Value.Hour < 8)
                try
                {
                    return (date.Value - new TimeSpan(1, 0, 0, 0)).ToString("dddd, dd. MMMM yyyy");
                }
                catch
                {
                    return date.Value.ToString("dddd, dd. MMMM yyyy");
                }
            else
                return date.Value.ToString("dddd, dd. MMMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidOperationException("DateTimeToEveningConverter: No backward conversion allowed.");
        }
    }
}
