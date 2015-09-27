using Bierstrichler.Data.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Enums
{
    /// <summary>
    /// Flags für Rechte der Person
    /// </summary>
    [Serializable]
    public class Rights : Serializable
    {
        public Rights()
        {
            Guest = true;
        }

        public Rights(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
        }

        /// <summary>
        /// Auf ihn darf gestrichelt werden
        /// </summary>
        [XmlAttribute]
        public bool Guest {get;set;}
        /// <summary>
        /// Darf selber stricheln
        /// </summary>
        [XmlAttribute]
        public bool Worker {get;set;}
        /// <summary>
        /// Darf Ein- und Auszahlungen tätigen
        /// </summary>
        [XmlAttribute]
        public bool Cashier {get;set;}
        /// <summary>
        /// Darf Personen Hinzufügen oder löschen
        /// </summary>
        [XmlAttribute]
        public bool Moderator {get;set;}
        /// <summary>
        /// Darf die Getränkeliste verwalten 
        /// </summary>
        [XmlAttribute]
        public bool BeerAdmin {get;set;}
        /// <summary>
        /// Darf das Programm Administrieren
        /// </summary>
        [XmlAttribute]
        public bool Administrator { get; set; }

        public override object Clone()
        {
 	        throw new NotImplementedException();
        }
    }
}
