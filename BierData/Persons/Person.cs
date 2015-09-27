using Bierstrichler.Data.Serialization;
using Bierstrichler.Data.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Bierstrichler.Data.Global;

namespace Bierstrichler.Data.Persons
{
    [Serializable]
    [XmlInclude(typeof(Consumer))]
    [XmlInclude(typeof(Special))]
    public abstract class Person : Serializable
    {
        public Person()
        {
            Adresse = new Address();
            Telefon = new PhoneNumber();
            Rights = new Rights();
            ID = Guid.NewGuid();
        }

        public Person(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
        }
        /// <summary>
        /// Clone Constructor.
        /// </summary>
        /// <param name="p">Person to clone</param>
        public Person(Person origin)
        {
            GetProperties(origin);
        }

        #region Properties

        /// <summary>
        /// Global Unique Identifier zum Zuweisen der Person beim Speichern
        /// </summary>
        [XmlAttribute]
        public Guid ID { get; set; }

        /// <summary>
        /// Vorname der Person
        /// </summary>
        [XmlAttribute]
        public string Vorname { get; set; }

        /// <summary>
        /// Nachname der Person
        /// </summary>
        [XmlAttribute]
        public string Nachname { get; set; }

        /// <summary>
        /// Email der Person
        /// </summary>
        [XmlAttribute]
        public string Email { get; set; }

        /// <summary>
        /// Telefonnummer der Person
        /// </summary>
        public PhoneNumber Telefon { get; set; }

        /// <summary>
        /// Adresse der Person
        /// </summary>
        public Address Adresse { get; set; }

        /// <summary>
        /// Geschlecht der Person
        /// </summary>
        [XmlAttribute]
        public Gender Geschlecht { get; set; }

        /// <summary>
        /// Relativer Pfad zum Bild des Gegenstands
        /// </summary>
        public string RelativeImagePath { get; set; }

        /// <summary>
        /// Image Uri
        /// </summary>
        [IgnoreProperty]
        public Uri ImageUri
        {
            get
            {
                if (RelativeImagePath != null)
                    return new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), RelativeImagePath), UriKind.Absolute);
                else
                    return null;
            }
        }

        /// <summary>
        /// Bereich, der vom Bild dargestellt werden soll.
        /// </summary>
        public Rectangle ImageCutOut { get; set; }

        /// <summary>
        /// Passwort Hash
        /// </summary>
        [XmlAttribute]
        public Byte[] Password { get; set; }

        /// <summary>
        /// Temporärer Tag für die Person (wird nicht gespeichert!)
        /// </summary>
        [IgnoreProperty]
        [XmlIgnore]
        public object Tag { get; set; }

        /// <summary>
        /// Vollständiger Name = Vorname + Nachname
        /// </summary>
        [IgnoreProperty]
        public string Name
        {
            get
            {
                return Vorname + " " + Nachname;
            }
        }

        /// <summary>
        /// Rechte des Nutzers
        /// </summary>
        public Rights Rights { get; set; }

        #endregion Properties

        public override string ToString()
        {
            return Name + "(" + this.GetType().Name + ")";
        }

        [IgnoreProperty]
        public string Initials
        {
            get
            {
                return (Vorname == null ? "" : new String(Vorname[0], 1)) + (Nachname == null ? "" : new String(Nachname[0],1));
            }
        }
    }
}
