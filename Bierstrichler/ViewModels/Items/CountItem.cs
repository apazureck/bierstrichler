using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bierstrichler.ViewModels.Items
{
    public class CountItem : ViewModelBase
    {
        private ItemViewModel Model;

        public CountItem(ItemViewModel i)
        {
            this.Model = i;
            Stock = Model.Stock;
        }

        private decimal formerBuyingPrize;

        public decimal FormerBuyingPrice
        {
            get { return formerBuyingPrize; }
            set
            {
                formerBuyingPrize = value;
                RaisePropertyChanged();
            }
        }

        private decimal actualBuyingPrice;

        public decimal ActualBuyingPrice
        {
            get { return actualBuyingPrice; }
            set
            {
                actualBuyingPrice = value;
                RaisePropertyChanged();
            }
        }

        private decimal priceForPurchase;

        public decimal PriceForPurchase
        {
            get { return priceForPurchase; }
            set
            {
                priceForPurchase = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get { return Model.Category; }
            set
            {
                Model.Category = value;
                RaisePropertyChanged();
            }
        }

        public bool Available
        {
            get { return Model.Available; }
            set
            {
                Model.Available = value;
                RaisePropertyChanged();
            }
        }

        public bool Coleurfaehig
        {
            get { return Model.Coleurfaehig; }
            set
            {
                Model.Coleurfaehig = value;
                RaisePropertyChanged();
            }
        }

        public int Stock
        {
            get { return stock; }
            private set { stock = value; }
        }

        int stock;

        private int counted;

        public int Counted
        {
            get { return counted; }
            set
            {
                counted = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CountingDiff");
            }
        }

        private int added;

        public int Added
        {
            get { return added; }
            set
            {
                added = value;
                RaisePropertyChanged();
            }
        }

        private decimal total;

        public decimal Total
        {
            get { return total; }
            set
            {
                total = value;
                RaisePropertyChanged();
            }
        }

        public int ID
        {
            get { return Model.Id; }
        }

        public ItemViewModel Correspondant
        {
            get { return Model; }
        }

        /// <summary>
        /// Counting Difference
        /// </summary>
        public int CountingDiff
        {
            get { return Counted - Stock; }
        }
    }
}
