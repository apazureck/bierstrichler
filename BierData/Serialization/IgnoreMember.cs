using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.Data.Serialization
{
    /// <summary>
    /// Attribute ignores Member for serialization when using the Bierstrichler.Data.Serialization.Serializable base method. See <see cref="Bierstrichler.Data.Persons.Person"/> for an example (Tag Property)
    /// </summary>
    class IgnorePropertyAttribute : Attribute
    {
    }
}
