using Bierstrichler.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Persons.Korpos
{
    [Serializable]
    public class Charge
    {
        [XmlAttribute]
        public Chargen Typ { get; set; }

        public SemesterCode SemesterCode { get; set; }
    }
}
