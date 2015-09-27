using Bierstrichler.Data.Items;
using Bierstrichler.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.ViewModels.Items
{
    public class CategoryViewModel : ViewModelBase
    {
        public CategoryViewModel()
        {
            Name = "None";
            Items = new ObservableCollection<ItemViewModel>();
        }

        public CategoryViewModel(ItemViewModel i) : this()
        {
            this.Name = i.Category;
            Items.Add(i);
        }

        private void AddToViewModelCollection(ItemViewModel ivm)
        {
            AddToViewModelCollection(ivm);
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if(Items!=null)
                    foreach (ItemViewModel ivm in Items)
                        ivm.Category = name;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<ItemViewModel> Items { get; set; }

        public override string ToString()
        {
            return Name + " [" + Items.Count + "]";
        }

        public void Deselect()
        {
            //Selected = null;
        }
    }
}
