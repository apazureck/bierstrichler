using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Items
{
    /// <summary>
    /// Änderung an einem Item
    /// </summary>
    [Serializable]
    public class Change : Serializable
    {
        #region Constructors

        public Change() { }
        /// <summary>
        /// Constructor used for specifying an initiator only. Customer will be default Guid.
        /// </summary>
        /// <param name="Initiator">Initiator of the action.</param>
        /// <param name="Date">DateTime when the action was initiated.</param>
        /// <param name="Amount">Change amount</param>
        /// <param name="ID">Change number</param>
        public Change(Person Initiator, DateTime Date, int Amount, int ID)
        {
            this.Initiator = Initiator.ID;
            this.Customer = default(Guid);
            this.Date = Date;
            this.Amount = Amount;
            this.ID = ID;
        }
        /// <summary>
        /// Constructor for Buying or selling an Item
        /// </summary>
        /// <param name="Initiator">Initiator of the action.</param>
        /// <param name="Customer">Customer, who bought the article.</param>
        /// <param name="Date">DateTime when the action was initiated.</param>
        /// <param name="Amount">Change amount</param>
        /// <param name="ID">Change number</param>
        public Change(Person Initiator, Person Customer, DateTime Date, int Amount, int ID)
        {
            this.Initiator = Initiator.ID;
            this.Customer = Customer.ID;
            this.Date = Date;
            this.Amount = Amount;
            this.ID = ID;
        }
        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Streaming Context</param>
        public Change(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
        }

        public Change(Change origin)
        {
            GetProperties(origin);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Sets or gets the Change Number
        /// </summary>
        [XmlAttribute]
        public int ID { set; get; }
        /// <summary>
        /// Datum der Änderung
        /// </summary>
        [XmlAttribute]
        public DateTime Date { get; set; }
        /// <summary>
        /// Relative Änderung an dem Item
        /// </summary>
        [XmlAttribute]
        public int Amount { get; set; }
        /// <summary>
        /// Verantwortlicher, der die Änderung verursacht hat.
        /// </summary>
        [XmlAttribute]
        public Guid Initiator { get; set; }
        /// <summary>
        /// Empfänger des Items
        /// </summary>
        [XmlAttribute]
        public Guid Customer { get; set; }

        #endregion Properties

        public override string ToString()
        {
            return "Date " + Date.ToString() + "; Amount " + Amount.ToString();
        }

        public override object Clone()
        {
            return new Change(this);
        }
    }
}
