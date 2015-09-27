using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Persons;
using Bierstrichler.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Items
{
    [Serializable]
    public class Consumed
    {
        /// <summary>
        /// Deserialization Constructor, do not use!
        /// </summary>
        public Consumed() { }

        /// <summary>
        /// Constructor for buying an item. The constructor will remove one unit of the item.
        /// </summary>
        /// <param name="item">Item to buy</param>
        /// <param name="price">Paid price for the item</param>
        /// <param name="Initiator">Person, who sold the item</param>
        /// <param name="Customer">Person, who bought the item</param>
        /// <param name="ID">Item ID</param>
        public Consumed(Item item, decimal price, Person Initiator, Person Customer, int ID)
        {
            ItemID = item.ID;
            Value = price;
            Type = ConsumeType.Purchase;
            ItemChangeID = item.Remove(Initiator, Customer);
            Date = DateTime.Now;
            Comment = "";
            this.ID = ID;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="amount">Amount of money to pay in, pay out or for special payment.</param>
        /// <param name="Initiator">Initiator who did the pay in</param>
        /// <param name="ID">Item ID</param>
        /// <param name="comment">Comment, fill out if special payment is intended.</param>
        public Consumed(decimal amount, Person Initiator, int ID, string comment = "")
        {
            ItemID = new Guid();
            Value = amount;
            if (comment == "")
            {
                if (amount < 0)
                    Type = ConsumeType.Inpayment;
                else
                    Type = ConsumeType.Payout;
                Comment = EnumHelper.GetDescription(Type);
            }
            else
            {
                Type = ConsumeType.SpecialPayment;
                if (amount < 0)
                    throw new Exception("Amount must not be negative with special payment.");
                Comment = comment;
            }
            this.ID = ID;
            Date = DateTime.Now;
            ItemChangeID = -1;
        }

        [XmlAttribute]
        public decimal Value { get; set; }

        [XmlAttribute]
        public Guid ItemID { get; set; }

        [XmlAttribute]
        public ConsumeType Type { get; set; }

        [XmlAttribute]
        public int ItemChangeID { get; set; }

        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public string Comment { get; set; }

        [XmlAttribute]
        public DateTime Date { get; set; }
    }
}
