using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Items;
using Bierstrichler.Data.Serialization;
using Bierstrichler.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Persons
{
    [Serializable]
    [XmlInclude(typeof(Gast))]
    [XmlInclude(typeof(Korpos.Korpo))]
    public abstract class Consumer : Person
    {
        public Consumer()
        {
            History = new List<Consumed>();
            ListedInCharts = true;
            LogoutBehavior = Enums.LogoutBehavior.Never;
        }
        /// <summary>
        /// Aktuelles Guthaben der Person
        /// </summary>
        [XmlAttribute]
        public decimal Guthaben { get; set; }

        /// <summary>
        /// Minimales Guthaben, das die Person haben darf (kann auch negativ sein)
        /// </summary>
        [XmlAttribute]
        public decimal MinGuthaben { get; set; }

        /// <summary>
        /// Guthaben, auf das aufgeladen werden soll, wenn die Person Autoload aktiviert hat
        /// </summary>
        [XmlAttribute]
        public decimal AutoloadGuthaben { get; set; }

        /// <summary>
        /// Reaktion, was zu tun ist, wenn das gewünschte Minimalguthaben unterschritten ist.
        /// </summary>
        [XmlAttribute]
        public ThresholdBehavior ThresholdBehavior { get; set; }

        /// <summary>
        /// Person wurde gesperrt und darf nicht gestrichelt werden.
        /// </summary>
        [XmlAttribute]
        public bool Gesperrt { get; set; }

        /// <summary>
        /// Person darf in negativen Guthabenbereich kommen.
        /// </summary>
        [XmlAttribute]
        public bool SkipNegativeSurcharge { get; set; }

        /// <summary>
        /// Person zahlt nur den Einkaufspreis.
        /// </summary>
        [XmlAttribute]
        public bool PaysBuingPrice { get; set; }

        /// <summary>
        /// Bestimmt das Autologout-Verhalten.
        /// </summary>
        public LogoutBehavior LogoutBehavior { get; set; }

        public void DepositMoney(decimal amount, Person Initiator)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Der Wert muss positiv sein.");
            Guthaben += amount;
            History.Add(new Consumed(-amount, Initiator, History.Count != 0 ? History.Last().ID + 1 : 0));
        }

        public void PayoutMoney(decimal amount, Person Initiator)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Der Wert muss positiv sein.");
            Guthaben -= amount;
            History.Add(new Consumed(amount, Initiator, History.Count != 0 ? History.Last().ID + 1 : 0));
        }

        public void SpecialPayment(decimal amount, Person Initiator, string Comment)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("Der Wert muss positiv sein.");
            Guthaben -= amount;
            History.Add(new Consumed(amount, Initiator, History.Count>0?History.Last().ID+1:0, Comment));
        }

        public List<Consumed> History { set; get; }

        public bool Pay(Items.Item item, decimal factor, Person Initiator)
        {
            decimal amount = PaysBuingPrice ? item.PriceBuying : item.PriceSelling * factor;
            Guthaben -= amount;
            History.Add(new Consumed(item, amount, Initiator, this, History.Count>0 ? History.Last().ID+1 : 0));
            return true;
        }

        /// <summary>
        /// Anzahl aller Drinks.
        /// </summary>
        [IgnoreProperty]
        public int TotalDrinks
        {
            get
            {
                return Task<int>.Run(() =>
                {
                    return History.FindAll(x => x.Type == ConsumeType.Purchase).Count;
                }).Result;
            }
        }

        /// <summary>
        /// Wenn diese Property aktiviert wird, wird die Person in den Biercharts gelistet.
        /// </summary>
        [XmlAttribute]
        public bool ListedInCharts { get; set; }

        [IgnoreProperty]
        public int TotalChartDrinks
        {
            get { return totalChartDrinks; }
        }

        private int totalChartDrinks;

        public void MakeTotalChartDrinks()
        {
            totalChartDrinks = BeerCharts.GetTotalChartDrinks(this);
        }

        /// <summary>
        /// Zeigt an, ob schon eine Email an den User gesandt wurde, falls er sich unter seinem MinGuthaben befindet und
        /// Email senden angefordert wurde.
        /// </summary>
        public bool MailWasSent { get; set; }

        public void RemoveHistoryChange(Consumed c)
        {
            History.Remove(c);
        }
    }
}
