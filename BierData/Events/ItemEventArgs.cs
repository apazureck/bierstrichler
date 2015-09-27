using Bierstrichler.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Events
{
    public class ItemEventArgs : EventArgs
    {
        public Item Item { get; set; }

        public int Index { get; set; }

        public ItemEventArgs(Item item = null)
        {
            this.Item = item;
            Index = -1;
        }

        public ItemEventArgs(Item item, int index)
        {
            this.Item = item;
            Index = index;
        }
    }
}
