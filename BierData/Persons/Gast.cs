using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Persons
{
    [Serializable]
    public class Gast : Consumer
    {
        public Gast() { }

        public Gast(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
        }

        public Gast(Person p)
        {
            GetProperties(p);
        }

        public override object Clone()
        {
            return new Gast(this);
        }
    }
}
