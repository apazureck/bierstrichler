using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Persons.Korpos
{
    [Serializable]
    public class Cartellbruder : Korpo
    {
        public Cartellbruder() : base()
        {

        }

        public Cartellbruder(SerializationInfo info, StreamingContext context) : base()
        {
            GetProperties(info, context);
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="p">Clone from Person.</param>
        public Cartellbruder(Person p)
        {
            GetProperties(p);
        }

        public override object Clone()
        {
            return new Cartellbruder(this);
        }
    }
}
