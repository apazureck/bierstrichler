using Bierstrichler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    public class AddressToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Address adresse = value as Address;
            if (adresse == null)
                return "Keine gültige Adresse";

            return adresse.ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string aStr = value as string;
            if (aStr == null)
                throw new ArgumentException("Argument has to be a string.", "value");

            return new Address(aStr);
        }
    }
}
