using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Functional
{
    public class DragDropContainer
    {
        public DragDropContainer(object data)
        {
            DropData = data;
            AllowDrop = true;
        }
        public object DropData { get; set; }

        public bool AllowDrop { get; set; }
    }
}
