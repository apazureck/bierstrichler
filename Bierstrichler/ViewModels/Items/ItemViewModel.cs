using Bierstrichler.Commands;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Items;
using Bierstrichler.Functional;
using Bierstrichler.Views.Items;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bierstrichler.ViewModels.Items
{
    public class ItemViewModel : ViewModelBase
    {
        public Item Model { get; private set; }

        private ItemDisplay View;

        public ItemViewModel(Item Model, ItemDisplay View) : this(Model)
        {
            this.View = View;
            View.DataContext = this;
        }

        public ItemViewModel(Item Model)
        {
            this.Model = Model;
            Model.ItemRemoved += Model_ItemRemoved;
            Changes = new ObservableCollection<Change>();
            foreach (Change c in Model.Changes)
                Changes.Add(c);
        }

        void Model_ItemRemoved(object sender, Data.Events.ChangeEventArgs e)
        {
            if (e.Change != null)
                Changes.Add(e.Change);
            else
            {
                Changes = new ObservableCollection<Change>();
                foreach (Change c in Model.Changes)
                    Changes.Add(c);
                RaisePropertyChanged("Changes");
            }
        }

        #region Properties
        public string Category
        {
            get { return Model.Category; }
            set
            {
                Model.Category = value;
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

        public int Id
        {
            get { return Model.Id; }
        }

        public string Description
        {
            get { return Model.Description; }
            set
            {
                Model.Description = value;
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

        public ImageSource Image
        {
            get
            {
                if(image==null)
                    return CreateNewImageSource();
                return image;
            }
            set
            {
                CreateNewImageSource();
                RaisePropertyChanged();
            }
        }
        BitmapImage image;

        private ImageSource CreateNewImageSource()
        {
            if (Model.RelativeImagePath != null)
            {
                BitmapImage cb;
                try
                {
                    cb = new BitmapImage(Model.ImageUri);
                    return cb;
                }
                catch (Exception)
                {
                    try
                    {
                        string lpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "images", "items", Path.GetFileName(Model.RelativeImagePath));
                        cb = new BitmapImage(new Uri(lpath, UriKind.Absolute));
                        Model.RelativeImagePath = lpath;
                    }
                    catch (Exception)
                    {
                        Model.RelativeImagePath = null;
                        cb = new BitmapImage();
                    }
                    App.SavePersonsListAsync();
                    return cb;
                }
            }
            else
                return new BitmapImage();

        }

        public int Stock
        {
            get { return Model.Stock; }
        }

        public decimal PriceBuying
        {
            get { return Model.PriceBuying; }
            set
            {
                if (ComparePrices(value, PriceSelling))
                    Model.PriceBuying = value;
                RaisePropertyChanged();
            }
        }

        public decimal PriceSelling
        {
            get { return Model.PriceSelling; }
            set
            {
                if(ComparePrices(PriceBuying, value))
                    Model.PriceSelling = value;
                    
                RaisePropertyChanged();
            }
        }

        bool ComparePrices(decimal newPriceBuying, decimal newPriceSelling)
        {
            if (newPriceBuying > newPriceSelling)
                return MessageBox.Show("Der Einkaufspreis ist höher als der Verkaufspreis!\nWollen Sie den Preis wirklich übernehmen?", "Achtung! Preise könnten falsch sein.", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
            else
                return true;
        }

        public ObservableCollection<Change> Changes { get; set; }

        private int amountAdded;

        public int AmountAdded
        {
            get { return amountAdded; }
        }

        private int amountRemoved;

        public int AmountRemoved
        {
            get { return amountRemoved; }
        }

        #endregion Properties

        #region Commands

        private ICommand removeUnitsCommand;
        public ICommand RemoveUnitsCommand
        {
            get
            {
                if (removeUnitsCommand == null)
                    removeUnitsCommand = new RelayCommand(param => RemoveUnits_Command(param));
                return removeUnitsCommand;
            }
            set
            {
                removeUnitsCommand = value;
            }
        }

        private void RemoveUnits_Command(object param)
        {
            RemoveUnits(Convert.ToUInt32(param));
        }

        public void RemoveUnits(uint amount)
        {
            Model.Remove(App.CurrentVendor, amount);
            //Changes.Add(Model.Changes.Last());
            RaisePropertyChanged("Stock");
            App.SaveItemListAsync();
        }

        private ICommand addUnitsCommand;
        public ICommand AddUnitsCommand
        {
            get
            {
                if (addUnitsCommand == null)
                    addUnitsCommand = new RelayCommand(param => AddNewUnits_Command(param));
                return addUnitsCommand;
            }
            set
            {
                addUnitsCommand = value;
            }
        }

        private void AddNewUnits_Command(object param)
        {
            AddUnits(Convert.ToUInt32(param));
        }

        public void AddUnits(uint amount)
        {
            Model.Add(App.CurrentVendor, amount);
            Changes.Add(Model.Changes.Last());
            RaisePropertyChanged("Stock");
            App.SaveItemListAsync();
        }

        private ICommand addImageCommand;

        public ICommand AddImageCommand
        {
            get
            {
                if (addImageCommand == null)
                    addImageCommand = new RelayCommand(param => AddImage_Command(param));
                return addImageCommand;
            }
            set
            {
                addImageCommand = value;
            }
        }

        private void AddImage_Command(object param)
        {
            // Configure open file dialog box 
            FileDialog dlg = DialogFactory.GetNewImageFileDialog();

            // Process open file dialog box results 
            if (dlg.ShowDialog() == true)
            {
                // Add Image Uri to Model.
                Model.RelativeImagePath = App.AddToItemImages(dlg.FileName);
                CreateNewImageSource();
                // Raise property changed to update the image.
                RaisePropertyChanged("Image");
            }
        }

        #endregion Commands

        override protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            Log.WriteInformation(App.CurrentVendor.Name + " changed " + propertyName + " of " + Name + " to " + this.GetType().GetProperty(propertyName).GetValue(this, null).ToString());
            base.RaisePropertyChanged(propertyName);
            App.SaveItemListAsync();
        }

        public override string ToString()
        {
            return Model.ToString();
        }

        public async void  CalculateChanges(DateTime startDate, DateTime endDate)
        {
            await Task.Run(() =>
                {
                    amountAdded = 0;
                    amountRemoved = 0;
                    filteredChanges.Clear();
                    foreach(Change c in Model.Changes)
                    {
                        if(c.Date >= startDate && c.Date <= endDate)
                        {
                            if (c.Amount > 0)
                                amountAdded += c.Amount;
                            else if (c.Amount < 0)
                                amountRemoved -= c.Amount;
                            filteredChanges.Add(c);
                        }
                    }
                });
            RaisePropertyChanged("AmountAdded");
            RaisePropertyChanged("AmountRemoved");
            RaisePropertyChanged("FilteredChanges");
        }

        private List<Change> filteredChanges = new List<Change>();
        public ObservableCollection<Change> FilteredChanges
        {
            get { return new ObservableCollection<Change>(filteredChanges); }
        }
    }
}
