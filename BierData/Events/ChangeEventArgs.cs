using Bierstrichler.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Events
{
    public class ChangeEventArgs : EventArgs
    {
        public Change Change { get; set; }
        public ChangeEventArgs(Change change = null)
        {
            this.Change = change;
        }
    }
}
