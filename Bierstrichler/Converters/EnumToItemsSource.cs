using Bierstrichler.Functional;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Bierstrichler.Converters
{
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Array a = Enum.GetValues(_type);
            List<String> l = new List<String>();
            foreach (Enum value in a)
                l.Add(EnumHelper.GetDescription(value));
            return l;
        }
    }
}
