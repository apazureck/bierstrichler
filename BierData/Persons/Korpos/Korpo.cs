using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Persons.Korpos
{
    [Serializable]
    [XmlInclude(typeof(Bundesbruder))]
    [XmlInclude(typeof(Cartellbruder))]
    public class Korpo : Consumer
    {
        public Korpo() : base()
        {

        }

        public Korpo(SerializationInfo info, StreamingContext context) : base()
        {
            GetProperties(info, context);
        }

        /// <summary>
        /// Clone constructor
        /// </summary>
        /// <param name="p">Clone from Person.</param>
        public Korpo(Person p) : base()
        {
            GetProperties(p);
        }

        /// <summary>
        /// Bünde des Korporierten
        /// </summary>
        public Bund[] Bund { get; set; }

        /// <summary>
        /// Vulgo des Korporierten
        /// </summary>
        public string Vulgo { get; set; }

        /// <summary>
        /// Status des Korporierten
        /// </summary>
        [XmlAttribute]
        public Status Status { get; set; }

        /// <summary>
        /// Rezeptionsdatum des Korporierten.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public DateTime? Rezeptionsdatum { get; set; }

        public List<Charge> Chargen { get; set; }

        [IgnoreProperty]
        public Charge AktuelleCharge
        {
            get
            {
                return null;
            }
        }

        public override object Clone()
        {
            return new Korpo(this);
        }
    }
}
