using Bierstrichler.Data.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Events
{
    public class PersonEventArgs : EventArgs
    {
        public Person Person { get; set; }

        public int Index { get; set; }

        public PersonEventArgs(Person person = null)
        {
            this.Person = person;
            Index = -1;
        }

        public PersonEventArgs(Person person, int index)
        {
            this.Person = person;
            Index = index;
        }
    }
}
