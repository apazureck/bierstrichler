using Bierstrichler.Data.Serialization;
using Bierstrichler.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Persons.Korpos
{
    [Serializable]
    public class Bundesbruder : Korpo
    {
        public Bundesbruder() : base() { }

        public Bundesbruder(SerializationInfo info, StreamingContext context)
            : base()
        {
            GetProperties(info, context);
        }
        /// <summary>
        /// Clone Constructor
        /// </summary>
        /// <param name="source">source to get values from</param>
        public Bundesbruder(Person source)
            : base()
        {
            GetProperties(source);
        }

        public override object Clone()
        {
            return new Bundesbruder(this);
        }
    }
}
