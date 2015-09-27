using Bierstrichler.ViewModels;
using Bierstrichler.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Events
{
    internal class SelectedItemChangedEventArgs : EventArgs
    {
        internal ItemViewModel Selected { set; get; }
        internal SelectedItemChangedEventArgs(ItemViewModel ivm = null)
        {
            this.Selected = ivm;
        }
    }
}
