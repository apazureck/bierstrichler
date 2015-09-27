using Bierstrichler.Data.Items;
using Bierstrichler.Data.Persons;
using Bierstrichler.ViewModels.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Functional
{
    public class DashBoard
    {
        public PersonList AllPersons { get; set; }

        public DashBoard(PersonList AllPersons)
        {
            this.AllPersons = AllPersons;
            PresentPersons = new List<Person>();
        }

        public IList<Person> PresentPersons { get; set; }

        public ItemList Items 
        {
            get
            {
                return App.Items;
            }
            set 
            { 
                App.Items = value; 
            }
        }
    }
}
