using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bierstrichler.Data.Items
{
    /// <summary>
    /// Ein Item, das verkauft wird.
    /// </summary>
    [Serializable]
    public class Item : Bierstrichler.Data.Serialization.Serializable
    {
        public event ItemChangedEvent ItemRemoved;

        #region Constructors

        /// <summary>
        /// Default constructor. Sets the guid and adds the change list.
        /// </summary>
        public Item() 
        {
            Changes = new List<Change>();
            ID = Guid.NewGuid();
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
        /// <param name="context">The destination <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.</param>
        public Item(SerializationInfo info, StreamingContext context)
        {
            GetProperties(info, context);
            stockChanged = true;
        }

        public Item(Item origin)
        {
            GetProperties(origin);
            stockChanged = true;
        }

        #endregion Constructors

        #region Fields

        /// <summary>
        /// Signalisiert, dass der Stock sich geändert hat.
        /// </summary>
        private bool stockChanged;

        #endregion Fields

        #region Properties
        /// <summary>
        /// Name des Gegenstands
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// ID des Gegenstands
        /// </summary>
        [XmlAttribute]
        public Guid ID { get; set; }
        /// <summary>
        /// Beschreibung des Gegenstands
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Relativer Pfad zum Bild des Gegenstands
        /// </summary>
        [XmlAttribute]
        public string RelativeImagePath { get; set; }
        /// <summary>
        /// Einkaufspreis
        /// </summary>
        [XmlAttribute]
        public decimal PriceBuying { get; set; }
        /// <summary>
        /// Ausgabepreis
        /// </summary>
        [XmlAttribute]
        public decimal PriceSelling { get; set; }
        /// <summary>
        /// Anzahl der noch vorhandenen Gegenstände
        /// </summary>
        [IgnoreProperty]
        public int Stock
        {
            get
            {
                if (stockChanged)
                    RecalculateStock();
                return stock;
            }
        }
        private int stock = 0;
        /// <summary>
        /// Änderungen an der Anzahl der verfügbaren Items
        /// </summary>
        public List<Change> Changes { get; set; }
        /// <summary>
        /// Kategorie des Items
        /// </summary>
        [XmlAttribute]
        public string Category { get; set; }
        /// <summary>
        /// Item wird in der Strichelliste angezeigt
        /// </summary>
        [XmlAttribute]
        public bool Available { get; set; }
        /// <summary>
        /// Signalisiert, ob der Stoff coleurfaehig ist.
        /// </summary>
        [XmlAttribute]
        public bool Coleurfaehig { get; set; }
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

        #endregion Properties

        override public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        /// <summary>
        /// Fügt ein Item dem Stock hinzu
        /// </summary>
        /// <param name="Initiator">Verantwortlicher für die Änderung</param>
        public void Add(Person Initiator)
        {
            Add(Initiator, 1);
        }
        /// <summary>
        /// Fügt eine gewünschte Anzahl zum Stock hinzu.
        /// </summary>
        /// <param name="Initiator">Verantwortlicher für die Änderung</param>
        /// <param name="Amount">Anzahl der Items</param>
        public void Add(Person Initiator, uint Amount)
        {
            Changes.Add(new Change(Initiator, DateTime.Now, Convert.ToInt32(Amount), Changes.Count>0? Changes.Last().ID+1 : 0));
            UpdateStock(Convert.ToInt32(Amount));
            Log.WriteInformation(Initiator.Name + " added " + Amount.ToString() + " " + this.Name);
        }
        /// <summary>
        /// Entfernt ein Item aus dem Stock
        /// </summary>
        /// <param name="Initiator">Verantwortlicher für die Änderung</param>
        public int Remove(Person Initiator)
        {
            return Remove(Initiator, 1);
        }
        /// <summary>
        /// Standard Strichelfunktion, entfernt 1 Item aus der Liste
        /// </summary>
        /// <param name="Initiator">Verkäufer</param>
        /// <param name="Customer">Käufer</param>
        public int Remove(Person Initiator, Person Customer)
        {
            return Remove(Initiator, Customer, 1);
        }
        /// <summary>
        /// Entfernt eine angegebene Anzahl an Items aus dem Stock.
        /// </summary>
        /// <param name="Initiator">Verantwortlicher für die Änderung</param>
        /// <param name="Amount">Anzahl der zu enfernenden Items (positiv)</param>
        public int Remove(Person Initiator, uint Amount)
        {
            Change c = new Change(Initiator, DateTime.Now, -Convert.ToInt32(Amount), Changes.Count > 0 ? Changes.Last().ID + 1 : 0);
            Changes.Add(c);
            UpdateStock(-Convert.ToInt32(Amount));
            Log.WriteInformation(Initiator.Name + " removed " + Amount.ToString() + " " + this.Name);
            if(ItemRemoved!=null)
                ItemRemoved(this, new ChangeEventArgs(c));
            return c.ID;
        }
        /// <summary>
        /// Funktion zum Verkaufen.
        /// </summary>
        /// <param name="Initiator">Verkäufer</param>
        /// <param name="Customer">Käufer</param>
        /// <param name="Amount">Anzahl</param>
        public int Remove(Person Initiator, Person Customer, uint Amount)
        {
            Change c = new Change(Initiator, Customer, DateTime.Now, -Convert.ToInt32(Amount), Changes.Count > 0 ? Changes.Last().ID + 1 : 0);
            Changes.Add(c);
            UpdateStock(-Convert.ToInt32(Amount));
            Log.WriteInformation(Initiator.Name + " sold " + Amount.ToString() + " of " + this.Name + " to " + Customer.Name);
            if (ItemRemoved != null)
                ItemRemoved(this, new ChangeEventArgs(c));
            return c.ID;
        }
        /// <summary>
        /// Updated den Stock ohne ihn neu zu berechnen.
        /// </summary>
        /// <param name="Amount">Änderung des Stocks</param>
        private void UpdateStock(int Amount)
        {
            if(stockChanged)
                RecalculateStockAsync();
            else
            {
                stock += Amount;
            }
        }
        /// <summary>
        /// Berechnet den Stock neu
        /// </summary>
        private void RecalculateStock()
        {
            stock = 0;
            foreach (Change c in Changes)
            {
                stock += c.Amount;
            }
            //TODO: Bug, dass man auch Items ins Negative stricheln kann.
            if (stock < 0)
                //throw new ArgumentException("Gesamtstock darf nicht kleiner Null werden. Resultierender Stock: " + stock.ToString(), "Stock");
            stockChanged = false;
        }
        /// <summary>
        /// Berechnet den Stock asynchron neu.
        /// </summary>
        private async void RecalculateStockAsync()
        {
            await Task.Run(
                () =>
                {
                    RecalculateStock();
                });
        }

        public override string ToString()
        {
            return Name + " (" + Stock + ") Category: " + Category + " Buy: " + PriceBuying + " Sell: " + PriceSelling;
        }

        public override object Clone()
        {
            return new Item(this);
        }

        public void RemoveHistoryChange(Consumed c)
        {
            Changes.Remove(Changes.Find(x=> x.ID == c.ItemChangeID));
            stockChanged = true;
            RecalculateStockAsync();
            if (ItemRemoved != null)
                ItemRemoved(this, new ChangeEventArgs());
        }
    }
}
