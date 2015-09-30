using Bierstrichler.Commands;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Persons.Korpos;
using Bierstrichler.Views.Persons;
using Bierstrichler.Data.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Bierstrichler.Functional;
using Bierstrichler.Events;
using GongSolutions.Wpf.DragDrop;
using Bierstrichler.Data.Events;
using Bierstrichler.ViewModels.Items;

//TODO: Abändern auf eine einzelne Liste und dann mit Styles groupen usw.
namespace Bierstrichler.ViewModels.Persons
{
    public class PersonListViewModel : ViewModelBase, IDropTarget, IDragSource
    {
        public event PersonViewModelChangedEventHandler SelectionChanged;
        public event PersonViewModelChangedEventHandler PersonRemovedByDrop;
        protected IList<Person> Model;

        private bool requestPasswordOnAdd;

        #region Properties

        public ObservableCollection<PersonViewModel> Persons { get; set; }

        private PersonViewModel selected;

        public PersonViewModel Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    if(SelectionChanged != null)
                        SelectionChanged(this, new PersonViewModelEventArgs(value));
                    selected = value;
                    RaisePropertyChanged();
                }
                    
            }
        }

        #endregion Properties

        private bool showCategories = true;
        /// <summary>
        /// Zeigt Kategorien auf Personenliste
        /// </summary>
        public bool ShowCategories
        {
            get { return showCategories; }
            set
            {
                showCategories = value;
                RaisePropertyChanged();
            }
        }

        public PersonListViewModel()
        {
            Persons = new ObservableCollection<PersonViewModel>();
        }

        public PersonListViewModel(IList<Person> Model) : this()
        {
            this.Model = Model;
            PersonList pl = Model as PersonList;
            if(pl != null)
            {
                pl.PersonAdded += pl_PersonAdded;
                pl.PersonRemoved += pl_PersonRemoved;
            }

            foreach(Person p in Model)
                AddViewModelToList(p);
        }

        void pl_PersonRemoved(object sender, PersonEventArgs e)
        {
            RemoveViewModelFromList(e.Person);
        }

        void pl_PersonAdded(object sender, PersonEventArgs e)
        {
            if(!IsAlreadyInList(e.Person))
                AddViewModelToList(e.Person);
        }

        private bool IsAlreadyInList(Person p)
        {
            return Persons.FirstOrDefault(x => x.Model == p) == default(PersonViewModel) ? false : true;
        }

        private void AddViewModelToList(Person p)
        {
            PersonViewModel pvm = new PersonViewModel(p);
            Persons.Add(pvm);
            pvm.PersonClassHasChanged += pvm_PersonClassHasChanged;
        }

        void pvm_PersonClassHasChanged(object sender, PersonEventArgs e)
        {
            //Persons.Remove((PersonViewModel)sender);
            //Persons.Add((PersonViewModel)sender);
            //RaisePropertyChangedForAll();
        }

        #region Commands

        private ICommand addNewPersonCommand;

        public ICommand AddNewPersonCommand
        {
            get
            {
                if (addNewPersonCommand == null)
                    addNewPersonCommand = new RelayCommand(param => addNewPerson_Command(param));
                return addNewPersonCommand;
            }
            set
            {
                addNewPersonCommand = value;
            }
        }

        private void addNewPerson_Command(object param)
        {
            AddNewPersonDialog dlg = new AddNewPersonDialog();
            bool? result = dlg.ShowDialog();
            if(result==true)
            {
                if(!Model.Contains(dlg.TempPerson.Model))
                    Model.Add(dlg.TempPerson.Model);
                BeerCharts.AddToCharts(dlg.TempPerson.Model as Consumer);
            }
            else if(result!=true)
                if(Model.Contains(dlg.TempPerson.Model))
                    Model.Remove(dlg.TempPerson.Model);
        }

        private ICommand removePersonCommand;

        public ICommand RemovePersonCommand
        {
            get
            {
                if (removePersonCommand == null)
                    removePersonCommand = new RelayCommand(param => RemovePerson_Command(param));
                return removePersonCommand;
            }
            set
            {
                removePersonCommand = value;
            }
        }

        private void RemovePerson_Command(object param)
        {
            Remove(Selected.Model);
        }


        #endregion

        internal void Add(Person p)
        {
            Model.Add(p);
            AddViewModelToList(p);
            RaisePropertyChanged("Count");
        }

        internal void Remove(Person p)
        {
            RemoveViewModelFromList(p);
            Model.Remove(p);
            RaisePropertyChanged("Count");
        }

        private void RemoveViewModelFromList(Person p)
        {
            try
            {
                Persons.Remove(Persons.First(x => x.Model == p));
            }
            catch { }
        }

        internal void LoginChanged()
        {
            RaisePropertyChangedForAll();
        }

        #region Drag'n'Drop

        public void DragOver(IDropInfo dropInfo)
        {
            DragDropContainer d = dropInfo.Data as DragDropContainer;
            if (d == null)
                return;
            if (d.DropData is PersonViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
            else if (d.DropData is ItemViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            DragDropContainer d = dropInfo.Data as DragDropContainer;
            
            if (d != null)
            {
                Consumer c = ((PersonViewModel)d.DropData).Model as Consumer;
                if (c == null)
                {
                    d.AllowDrop = false;
                    return;
                }
                RaisePropertyChanged("Count");
            }
            else
                throw new ArgumentException("dropInfo.Data has to be of Type DragDropContainer");
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void DragCancelled()
        {
        }

        public void Dropped(IDropInfo dropInfo)
        {
            DragDropContainer d = dropInfo.Data as DragDropContainer;
            if (d != null && d.AllowDrop)
            {
                if (d.DropData is PersonViewModel && PersonRemovedByDrop != null)
                    PersonRemovedByDrop(this, new PersonViewModelEventArgs((PersonViewModel)d.DropData));
                RaisePropertyChanged("Count");
            }
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            if(dragInfo.SourceItem is PersonViewModel)
            {
                dragInfo.Data = new DragDropContainer(dragInfo.SourceItem);
                dragInfo.Effects = System.Windows.DragDropEffects.Copy | System.Windows.DragDropEffects.Move;
            }
        }

        #endregion Drag'n'Drop

        public int Count
        {
            get { return Model.Count; }
        }
    }
}
