using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Persons
{
    [Serializable]
    public class Special : Consumer
    {
        public Special()
            : base()
        {

        }

        public Special(SerializationInfo info, StreamingContext context) : base()
        {
            GetProperties(info, context);
        }

        public Special(Person p) : base()
        {
            GetProperties(p);
        }

        public override object Clone()
        {
            return new Special(this);
        }
    }
}
