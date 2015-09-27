using Bierstrichler.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters.History
{
    public class ItemDateFromConsumedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Consumed))
                return null;
            Consumed c = value as Consumed;
            TimeSpan t = DateTime.Now.Subtract(c.Date);
            string ret = "vor ";
            if (t.Days < 1)
                if (t.Hours < 1)
                    if (t.Minutes < 1)
                        return ret + t.Seconds.ToString() + " sec";
                    else
                        return ret + (t.Minutes + t.Seconds / 60d).ToString("#.0") + " min";
                else
                    return ret + (t.Hours + t.Minutes / 60d).ToString("#.0") + " Stunden";
            else
                return ret + (t.Days + t.Hours / 24d).ToString("#.0") + " Tagen";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
