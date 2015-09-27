using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bierstrichler.Converters
{
    public sealed class BooleanToInvisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToInvisibilityConverter() : 
            base(Visibility.Collapsed, Visibility.Visible) {}
    }
}
