using Bierstrichler.Data.Items;
using Bierstrichler.Data.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Serialization
{
    /// <summary>
    /// Klasse ist ein Container für die XML Deserialisierung
    /// </summary>
    public class ListContainer
    {
        public ListContainer() { }

        public ListContainer(IList<Person> persons, IList<Item> items)
        {
            Persons = new List<Person>(persons);
            Items = new List<Item>(items);
        }

        public List<Person> Persons { get; set; }
        public List<Item> Items { get; set; }
    }
}
