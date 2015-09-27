using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bierstrichler.CustomControls
{
    public class RectangularWrapPanel : WrapPanel
    {
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if(Children!=null&&Children.Count>0)
            {
                if(Orientation==Orientation.Horizontal)
                    constraint.Width = Math.Ceiling(Math.Sqrt(Children.Count)) * (Properties.Settings.Default.ItemSize + 20.0);
                else
                    constraint.Height = Math.Ceiling(Math.Sqrt(Children.Count)) * (Properties.Settings.Default.ItemSize + 20.0);
            }
            return base.MeasureOverride(constraint);
        }

    }
}
