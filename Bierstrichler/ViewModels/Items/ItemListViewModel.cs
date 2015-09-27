using Bierstrichler.Commands;
using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Items;
using Bierstrichler.Events;
using Bierstrichler.Functional;
using Bierstrichler.Views.Items;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Bierstrichler.ViewModels.Items
{
    public class ItemListViewModel : ViewModelBase, IDragSource
    {
        public event ItemChangedEventHandler ItemClicked;

        private IList<Item> Model;

        public IList<Item> GetModel() { return Model; }

        #region Properties
        public ObservableCollection<ItemViewModel> Items {
            get { return items; }
            set 
            { 
                items = value;
                RaisePropertyChanged();
            }
        }
        ObservableCollection<ItemViewModel> items;

        public ObservableCollection<CategoryViewModel> Categories { set; get; }

        private ItemViewModel selectedItem;

        public ItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public object Selected
        {
            get
            {
                return SelectedItem;
            }
            set
            {
                if(value == null)
                {
                    //if (selectedItem != null)
                    //    selectedItem.RemoveView();
                    SelectedItem = null;
                    RaisePropertyChangedForAll();
                }
                else if (value is ItemViewModel)
                {
                    if (SelectedItem != null)
                        SelectedItem = null;
                    SelectedItem = (ItemViewModel)value;
                    //selectedItem.AddView(View.ItemDisplay);
                    SelectedCategory = Categories.FirstOrDefault(x => x.Name == SelectedItem.Category);                    
                    RaisePropertyChanged();
                }
                else
                {
                    SelectedCategory = (CategoryViewModel)value;
                    SelectedItem = null;
                }
                    
            }
        }

        public CategoryViewModel SelectedCategory
        {
            get;
            set;
        }

        #endregion Properties

        #region Constructor

        public ItemListViewModel(IList<Item> Model, bool sync = false)
        {
            this.Model = Model;
            if(Model is ItemList)
            {
                ((ItemList)Model).ItemAdded += ItemListViewModel_ItemAdded;
                ((ItemList)Model).ItemRemoved += ItemListViewModel_ItemRemoved;
            }
            Categories = new ObservableCollection<CategoryViewModel>();
            Items = new ObservableCollection<ItemViewModel>();
            if (sync)
                SetupItems(Model);
            else
                SetupItemsAsync(Model);
        }     

        protected virtual void ItemListViewModel_ItemRemoved(object sender, ItemEventArgs e)
        {
            foreach(ItemViewModel ivm in Items)
                if(ivm.Model == e.Item)
                {
                    Items.Remove(ivm);
                    break;
                }
            RaisePropertyChangedForAll();
        }

        protected virtual void ItemListViewModel_ItemAdded(object sender, ItemEventArgs e)
        {
            ItemViewModel ivm = new ItemViewModel(e.Item);
            AddToViewModelCollection(ivm);
            RaisePropertyChangedForAll();
        }

        private async void SetupItemsAsync(IList<Item> Model)
        {
            await Task.Run(() =>
            {
                SetupItems(Model);
            });
        }

        private void SetupItems(IList<Item> Model)
        {
            SetProgressBar("Setting up itemlist", 0.0);
            int i = 0;
            foreach (Item item in Model)
            {
                AddNewItem(item);
                UpdateProgress(i++, Model.Count);
            }
            SetProgressBar("Items list updated", 100.0);
            ResetProgressBar(1000);
            RaisePropertyChangedForAll();
        }

        #endregion Constructor

        #region Commands

        private ICommand addNewItemCommand;
        public ICommand AddNewItemCommand
        {
            get
            {
                if(addNewItemCommand==null)
                    addNewItemCommand = new RelayCommand(param => AddNewItem_Command(param));
                return addNewItemCommand;
            }
            set
            {
                addNewItemCommand = value;
            }
        }

        void AddNewItem_Command(object param)
        {
            if (Categories.Count<1)
            {
                AddNewCategory();
                SelectedCategory = Categories[0];
            }
            Item i = new Item()
            {
                Name = "Neues Item",
                Description = "Beschreibung",
                Category = SelectedCategory.Name
            };
            Model.Add(i);
            App.SaveItemListAsync();
            AddNewItem(i);
        }

        private ICommand addNewCategoryCommand;

        public ICommand AddNewCategoryCommand
        {
            get
            {
                if (addNewCategoryCommand == null)
                    addNewCategoryCommand = new RelayCommand(param => AddNewCategory_Command(param));
                return addNewCategoryCommand;
            }
            set
            {
                addNewCategoryCommand = value;
            }
        }

        void AddNewCategory_Command(object param)
        {
            AddNewCategory();
            App.SaveItemListAsync();
        }

        private ICommand removeItemCommand;

        public ICommand RemoveItemCommand
        {
            get
            {
                if (removeItemCommand == null)
                    removeItemCommand = new RelayCommand(param => RemoveItem_Command(param));
                return removeItemCommand;
            }
            set
            {
                removeItemCommand = value;
            }
        }

        private void RemoveItem_Command(object param)
        {
            Model.Remove(SelectedItem.Model);
            Items.Remove(SelectedItem);
            SelectedCategory.Items.Remove(SelectedItem);
            Selected = null;
            App.SaveItemListAsync();
        }

        private ICommand buyCommand;

        public ICommand BuyCommand
        {
            get
            {
                if (buyCommand == null)
                    buyCommand = new RelayCommand(param => Buy_Command(param));
                return buyCommand;
            }
            set
            {
                buyCommand = value;
            }
        }

        private void Buy_Command(object param)
        {
            if (!(param is ItemViewModel))
                return;
            if (ItemClicked != null)
                ItemClicked(this, new ItemEventArgs(((ItemViewModel)param).Model as Item));
        }

        private ICommand countItemsCommand;

        public ICommand CountItemsCommand
        {
            get
            {
                if (countItemsCommand == null)
                    countItemsCommand = new RelayCommand(param => CountItems_Command(param));
                return countItemsCommand;
            }
            set
            {
                countItemsCommand = value;
            }
        }

        private void CountItems_Command(object param)
        {
            CountItemsDialog cid = new CountItemsDialog(this);
            if(cid.ShowDialog() == true)
            {
                Log.WriteDebug("CountItemsDialog returned true.");
            }
        }


        #endregion Commands

        #region Methods

        private CategoryViewModel AddToMatchingCategory(ItemViewModel ivm)
        {
            CategoryViewModel cvm = Categories.FirstOrDefault(x => x.Name == ivm.Category);
            if (cvm == null)
            {
                AddNewCategory(ivm);
            }
            else
                cvm.Items.Add(ivm);
            SelectedCategory = cvm;
            return cvm;
        }

        private void AddNewCategory(ItemViewModel ivm = null)
        {                
            CategoryViewModel cvm = ivm != null ? new CategoryViewModel(ivm) : new CategoryViewModel();
            Categories.Add(cvm);
        }

        private ItemViewModel AddNewItem(Item i)
        {
            ItemViewModel ivm = new ItemViewModel(i);
            ivm.PropertyChanged += ivm_PropertyChanged;
            AddToMatchingCategory(ivm);
            AddToViewModelCollection(ivm);
            return ivm;
        }

        private ICommand filterTimeRangeCommand;

        public ICommand FilterTimeRangeCommand
        {
            get
            {
                if (filterTimeRangeCommand == null)
                    filterTimeRangeCommand = new RelayCommand(param => FilterTimeRange_Command(param));
                return filterTimeRangeCommand;
            }
            set
            {
                filterTimeRangeCommand = value;
            }
        }

        private void FilterTimeRange_Command(object param)
        {
            foreach (ItemViewModel ivm in Items)
                ivm.CalculateChanges(startDate, endDate);
        }

        private DateTime startDate = DateTime.Today - new TimeSpan(7,12,0,0);

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value + new TimeSpan(12,0,0);
                RaisePropertyChanged();
            }
        }

        private DateTime endDate = DateTime.Now;

        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value + new TimeSpan(12, 0, 0);
                RaisePropertyChanged();
            }
        }

        void ivm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void AddToViewModelCollection(ItemViewModel ivm)
        {
            try
            {
                Items.Add(ivm);
            }
            catch { }
        }

        public override string ToString()
        {
            return Model.ToString();
        }

        #endregion Methods

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void DragCancelled()
        {
            
        }

        public void Dropped(IDropInfo dropInfo)
        {
            if(dropInfo.Data is ItemViewModel)
            {
                int i = 0;
            }
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is ItemViewModel)
            {
                dragInfo.Data = dragInfo.SourceItem;
                dragInfo.Effects = System.Windows.DragDropEffects.Copy | System.Windows.DragDropEffects.Move;
            }
        }
    }
}
