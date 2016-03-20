using Bierstrichler.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bierstrichler.Converters.History
{
    public class ItemNameFromConsumedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Consumed))
                return null;
            Consumed c = value as Consumed;
            switch(c.Type)
            {
                case Data.Enums.ConsumeType.Inpayment:
                    return "Einzahlung";
                case Data.Enums.ConsumeType.Payout:
                    return "Auszahlung";
                case Data.Enums.ConsumeType.Purchase:
                    Item i = App.Items.Find(x => x.Equals(c.Item));
                    if (i == null)
                        return "Fehler!";
                    return i.Name + " erworben";
                case Data.Enums.ConsumeType.SpecialPayment:
                    return c.Comment;
                default:
                    return "Fehler!";
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
