using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters
{
    class MailAddressToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MailAddress address = value as MailAddress;
            if (address == null)
                return "";

            return address.Address;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string aStr = value as String;
            if (string.IsNullOrEmpty(aStr))
                return null;

            return new MailAddress(aStr);
        }
    }
}
