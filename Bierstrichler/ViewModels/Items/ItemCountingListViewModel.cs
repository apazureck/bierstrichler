using Bierstrichler.Commands;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bierstrichler.ViewModels.Items
{
    public class ItemCountingListViewModel : ItemListViewModel
    {
        public event EventHandler CountingDone;
        public event EventHandler CountingCancelled;

        DateTime LastCountingDate
        {
            get { return Properties.Settings.Default.LastCounting; }
            set
            {
                Properties.Settings.Default.LastCounting = value;
                Properties.Settings.Default.Save();
            }
        }

        private FastObservableCollection<CountItem> countings = new FastObservableCollection<CountItem>();

        public FastObservableCollection<CountItem> Countings
        {
            get { return countings; }
            set
            {
                countings = value;
                RaisePropertyChanged();
            }
        }

        public ItemCountingListViewModel(IList<Item> Model) : base(Model, true)
        {
            for(int i = 0; i<Items.Count; i++ )
                Countings.Add(new CountItem(Items[i]));
        }

        #region CountingConfirmedCommand

        private ICommand countingConfirmedCommand;

        public ICommand CountingConfirmedCommand
        {
            get
            {
                if (countingConfirmedCommand == null)
                    countingConfirmedCommand = new RelayCommand(param => CountingConfirmed_Command(param));
                return countingConfirmedCommand;
            }
            set
            {
                countingConfirmedCommand = value;
            }
        }

        private void CountingConfirmed_Command(object param)
        {
            CountAllItems();
        }

        #endregion

        #region CancelCountingCommand

        private ICommand cancelCountingCommand;

        public ICommand CancelCountingCommand
        {
            get
            {
                if (cancelCountingCommand == null)
                    cancelCountingCommand = new RelayCommand(param => CancelCounting_Command(param));
                return cancelCountingCommand;
            }
            set
            {
                cancelCountingCommand = value;
            }
        }

        private void CancelCounting_Command(object param)
        {
            if (CountingDone != null)
                CountingCancelled(this, null);
        }

        #endregion CancelCountingCommand

        protected override void ItemListViewModel_ItemAdded(object sender, Data.Events.ItemEventArgs e)
        {
            base.ItemListViewModel_ItemAdded(sender, e);
            Countings.Add(new CountItem(Items.FirstOrDefault(x => x.ID == e.Item.ID)));
        }

        protected override void ItemListViewModel_ItemRemoved(object sender, Data.Events.ItemEventArgs e)
        {
            base.ItemListViewModel_ItemRemoved(sender, e);
            Countings.Remove(Countings.FirstOrDefault(x => x.ID == e.Item.ID));
        }

        private void CountAllItems()
        {
            int totalmissing = 0;
            int totalconsumed = 0;
            decimal totalLoss = 0;

            foreach (CountItem ci in Countings)
            {
                ci.FormerBuyingPrice = ci.Correspondant.PriceBuying;
                List<Change> cs = ci.Correspondant.Model.Changes.FindAll(x => x.Date > LastCountingDate);
                int changeAmount = 0;
                foreach (Change c in cs)
                    if (c.Amount < 0)
                        changeAmount -= c.Amount;
                totalconsumed += changeAmount;

                int lastAmount = changeAmount + ci.Stock;

                if (ci.Stock > ci.Counted)
                // Fehlstriche
                {
                    int missing = ci.Stock - ci.Counted;
                    totalmissing += missing;
                    totalLoss = missing * ci.Correspondant.PriceBuying;
                    ci.Correspondant.RemoveUnits(Convert.ToUInt32(missing));
                }
                else if (ci.Stock < ci.Counted)
                    // Zu viel gestrichelt
                    ci.Correspondant.AddUnits(Convert.ToUInt32(ci.Counted - ci.Stock));

                // Added new Order
                if (ci.Added > 0)
                {
                    ci.Correspondant.AddUnits(Convert.ToUInt32(ci.Added));
                    // Calc new Buying Price
                    if (ci.Total > 0)
                    {
                        ci.PriceForPurchase = ci.Total / ci.Added;
                        if (ci.Correspondant.PriceBuying > 0)
                        {
                            decimal total = lastAmount + ci.Added;
                            ci.Correspondant.PriceBuying = (ci.Total + lastAmount * ci.Correspondant.PriceBuying) / total;
                        }
                        else
                            ci.Correspondant.PriceBuying = ci.Total / ci.Added;
                        ci.ActualBuyingPrice = ci.Correspondant.PriceBuying;
                    }
                }

                ci.Available = ci.Stock > 0;
            }

            SaveCountingsToFile();

            LastCountingDate = DateTime.Now;
            double totalFailRate = (double)totalmissing / (double)totalconsumed;
            Log.WriteInformation("Fehlstrichquote beträgt " + totalFailRate.ToString("P2"));

            if (CountingDone != null)
                CountingDone(this, null);
        }

        private void SaveCountingsToFile()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Counted on " + DateTime.Now.ToString("yyyy-MM-dd") + "Last Counting" + LastCountingDate.ToString("yyyy-MM-dd"));
            sb.AppendLine("by " + App.CurrentVendor.Name);
            sb.AppendLine("ID, Name, OldStock, NewStock, Added");
            foreach (CountItem ci in Countings)
                sb.AppendLine(ci.ID.ToString() + ", " + ci.Name + ", " + ci.Stock + ", " + ci.Correspondant.Stock + ", " + ci.Added);

            string file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", "counting", DateTime.Now.ToString("yyyy-MM-dd") + "_Counting.txt");

            //Create new Filename with 1.2..3...4 usw.
            if (File.Exists(file))
            {
                string outfile = null;
                for (int i = 1; File.Exists(file); i++)
                    outfile = file + "_" + i.ToString();
                file = outfile;
            }

            File.WriteAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Bierstrichler","counting",DateTime.Now.ToString("yyyy-MM-dd") + "_Counting.txt"), sb.ToString());
        }
    }
}
