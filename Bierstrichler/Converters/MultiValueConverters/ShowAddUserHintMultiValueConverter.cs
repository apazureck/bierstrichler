using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bierstrichler.Converters.MultiValueConverters
{
    public class ShowAddUserHintMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool addButtonPressed = values[0] is bool ? (bool)values[0] : false;
            bool elementsInAddList = values[1] is int ? (int)values[1] > 0 : false;
            return addButtonPressed && !elementsInAddList ? Visibility.Visible : Visibility.Collapsed;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
