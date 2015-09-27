using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bierstrichler.TemplateSelectors
{
    class PersonListGroupStyleTemplateSelector : StyleSelector
    {
        public Style UserTypeStyle { get; set; }

        public Style UserStatusStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            GroupItem cp = container as GroupItem;

            if (cp != null)
            {
                Type t = typeof(GroupItem);
                PropertyInfo pi = t.GetProperty("Generator");
                ItemContainerGenerator g = pi.GetValue(cp) as ItemContainerGenerator;
                return  0 > 1 ? UserStatusStyle : UserTypeStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
