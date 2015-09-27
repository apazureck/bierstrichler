using Bierstrichler.Commands;
using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Persons.Korpos;
using Bierstrichler.Functional;
using Bierstrichler.ViewModels.Items;
using Bierstrichler.ViewModels.Persons;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Enums = Bierstrichler.Data.Enums;

namespace Bierstrichler.ViewModels
{
    public class DashListViewModel : ViewModelBase
    {
        private Functional.DashBoard Model;

        private StringCollection LoggedInUsers { get { return Properties.Settings.Default.LoggedInUsers; } }

        private Dictionary<Guid, DispatcherTimer> logoutTimers = new Dictionary<Guid, DispatcherTimer>();

        #region Properties

        public PersonListViewModel AllPersons { set; get; }

        public PersonListViewModel PresentPersons { set; get; }

        public BeerChartsViewModel BeerCharts { get; set; }

        public ItemListViewModel Items { get; set; }

        #region AddList

        private bool isAddListVisible;
        public bool IsAddListVisible
        {
            get { return isAddListVisible; }
            set
            {
                isAddListVisible = value;
                if (!isAddListVisible)
                {
                    AllPersons.Selected = null;
                    AddButtonText = "Hinzufügen";
                    addListCloseTimer.Stop();
                }
                else
                {
                    AddButtonText = "Ausblenden";
                    addListCloseTimer.Start();
                }
                RaisePropertyChanged();
            }
        }

        private string addButtonText = "Hinzufügen";

        public string AddButtonText
        {
            get { return addButtonText; }
            set
            {
                addButtonText = value;
                RaisePropertyChanged();
            }
        }

        DispatcherTimer addListCloseTimer;

        #endregion AddList

        #endregion Properties

        public DashListViewModel(DashBoard DashBoard)
        {
            Items = new ItemListViewModel(DashBoard.Items);
            SetupDashBoard(DashBoard);
        }

        public DashListViewModel(DashBoard DashBoard, ItemListViewModel AllItems)
        {
            this.Items = AllItems;
            SetupDashBoard(DashBoard);
        }

        private void SetupDashBoard(DashBoard DashBoard)
        {
            this.Model = DashBoard;
            Items.ItemClicked += Items_ItemClicked;
            
            if(DashBoard.AllPersons is PersonList)
            {
                ((PersonList)DashBoard.AllPersons).PersonAdded += PersonAdded;
                ((PersonList)DashBoard.AllPersons).PersonRemoved += PersonRemoved;
                AllPersons = new PersonListViewModel(new List<Person>(DashBoard.AllPersons));
            }
            else
                AllPersons = new PersonListViewModel(DashBoard.AllPersons);

            PresentPersons = new PersonListViewModel(DashBoard.PresentPersons);

            BeerCharts = new BeerChartsViewModel();

            App.CurrentVendorChanged += App_CurrentVendorChanged;

            // Close Timer einstellen.
            addListCloseTimer = new DispatcherTimer();
            addListCloseTimer.Interval = new TimeSpan(0, 0, 30);
            addListCloseTimer.Tick += (object sender, EventArgs e) =>
            {
                IsAddListVisible = false;
                addListCloseTimer.Stop();
            };
            addListCloseTimer.Start();

            AllPersons.SelectionChanged += (object sender, Events.PersonViewModelEventArgs e) =>
                {
                    if (addListCloseTimer.IsEnabled)
                    {
                        addListCloseTimer.Stop();
                        addListCloseTimer.Start();
                    }
                };

            AllPersons.PersonRemovedByDrop += AllPersons_PersonRemovedByDrop;
            PresentPersons.PersonRemovedByDrop += PresentPersons_PersonRemovedByDrop;
            CreatePresentListFromSavedFile();
        }

        void PresentPersons_PersonRemovedByDrop(object sender, Events.PersonViewModelEventArgs e)
        {
            RemovePersonFromPresentList(e.Person.Model);
        }

        void AllPersons_PersonRemovedByDrop(object sender, Events.PersonViewModelEventArgs e)
        {
            AddPersonToPresentList((Consumer)e.Person.Model);
        }

        private void CreatePresentListFromSavedFile()
        {
            List<string> loggedInUsers = new List<string>();
            foreach (string s in LoggedInUsers)
                loggedInUsers.Add(s);
            LoggedInUsers.Clear();
            Properties.Settings.Default.Save();
            foreach(string s in loggedInUsers)
            {
                string[] sv = s.Split(';');
                Person p = App.Persons.Find(x => x.ID == new Guid(sv[0]));
                DateTime LogoutDate = DateTime.Parse(sv[1]);
                if (p is Consumer && !((Consumer)p).Gesperrt)
                    AddPersonToPresentList((Consumer)p, LogoutDate);
            }
        }

        void Items_ItemClicked(object sender, ItemEventArgs e)
        {
            if (PresentPersons.Selected != null)
            {
                PresentPersons.Selected.Buy(e.Item);
                if(Bierstrichler.Properties.Settings.Default.QuestionGameActive)
                    QuestionMaster.Hit();
            }
        }

        #region EventHandlers

        void App_CurrentVendorChanged(object sender, PersonEventArgs e)
        {
            RaisePropertyChangedForAll();
        }

        void PersonRemoved(object sender, PersonEventArgs e)
        {
            try
            {
                PresentPersons.Remove(e.Person);
            }
            catch { }
            AllPersons.Remove(e.Person);
        }

        void PersonAdded(object sender, PersonEventArgs e)
        {
            AllPersons.Add(e.Person);
        }

        #endregion EventHandlers

        #region Commands

        #region movePersonToPresentList

        private ICommand movePersonToPresentList;
        public ICommand MovePersonToPresentList
        {
            get
            {
                if (movePersonToPresentList == null)
                    movePersonToPresentList = new RelayCommand(param => MovePersonToPresentList_Command(param));
                return movePersonToPresentList;
            }
            set
            {
                movePersonToPresentList = value;
            }
        }

        private void MovePersonToPresentList_Command(object param)
        {
            if (!(param is PersonViewModel))
                return;
            Consumer c = ((PersonViewModel)param).Model as Consumer;
            if (c == null)
                return;

            AddPersonToPresentList(c);
        }

        #endregion

        private void AddPersonToPresentList(Consumer c, DateTime logoutDate = default(DateTime))
        {
            if (c.Gesperrt)
            {
                LoginBox lb = new LoginBox(c.Name);
                if (!lb.ShowDialog() == true)
                    return;
            }
            PresentPersons.Add(c);
            AllPersons.Remove(c);
            if(logoutDate == default(DateTime))
                logoutDate = SetLogoutTime(c);
            Bierstrichler.Properties.Settings.Default.LoggedInUsers.Add(c.ID.ToString() + ";" + logoutDate.ToString());
            CreateLogoutTimer(c.ID, logoutDate);
            Bierstrichler.Properties.Settings.Default.Save();
            Log.WriteInformation(App.CurrentVendor.Name + " moved " + c.Name + " to Present List.");
        }

        private void CreateLogoutTimer(Guid guid, DateTime LogoutTime)
        {
            DispatcherTimer logoutTimer = new DispatcherTimer();
            TimeSpan spanTillLogout = LogoutTime.Subtract(DateTime.Now);
            try
            {
                logoutTimer.Interval = spanTillLogout;
                logoutTimer.Tick += logoutTimer_Tick;
                logoutTimer.Tag = guid;
                logoutTimer.Start();
                logoutTimers.Add(guid, logoutTimer);
            }
            catch
            {
                logoutTimers.Add(guid, null);
            }
        }

        void logoutTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer logoutTimer = sender as DispatcherTimer;
            if (logoutTimer == null)
                return;
            if (!(logoutTimer.Tag is Guid))
                return;
            Guid UserID = (Guid)logoutTimer.Tag;
            if(logoutTimers.ContainsKey(UserID))
                RemovePersonFromPresentList(App.Persons.Find(x => x.ID == UserID));
        }

        /// <summary>
        /// Legt den Logoutzeitpunkt für den Nutzer anhand seiner Einstellungen fest.
        /// </summary>
        /// <param name="lout"></param>
        public static DateTime SetLogoutTime(Consumer c)
        {
            DateTime lout = DateTime.Now;

            switch (c.LogoutBehavior)
            {
                case Enums.LogoutBehavior.Never:
                    return DateTime.MaxValue;
                case Enums.LogoutBehavior.OneHour:
                    return lout.AddHours(1);
                case Enums.LogoutBehavior.TenMinutes:
                    return lout.AddMinutes(10);
                case Enums.LogoutBehavior.TheNextMorning:
                    return new DateTime(lout.Year, lout.Month, lout.Hour >= 8 ? lout.Day + 1 : lout.Day, 8, 0, 0);
                case Enums.LogoutBehavior.ThirtyMinutes:
                    return lout.AddMinutes(30);
                case Enums.LogoutBehavior.ThreeHours:
                    return lout.AddHours(3);
                default:
                    throw new ArgumentException("No valid Logout Behavior");
            }
        }

        private ICommand removePersonFromPresentListCommand;
        public ICommand RemovePersonFromPresentListCommand
        {
            get
            {
                if (removePersonFromPresentListCommand == null)
                    removePersonFromPresentListCommand = new RelayCommand(param => RemovePersonFromPresentList_Command(param));
                return removePersonFromPresentListCommand;
            }
            set
            {
                removePersonFromPresentListCommand = value;
            }
        }

        private void RemovePersonFromPresentList_Command(object param)
        {
            if (!(param is PersonViewModel))
                return;
            Person p = ((PersonViewModel)param).Model as Person;

            RemovePersonFromPresentList(p);
        }

        private void RemovePersonFromPresentList(Person p)
        {
            // Remove from PresentList
            AllPersons.Add(p);
            PresentPersons.Remove(p);

            int foundIndex = -1;
            
            // remove Timer
            try
            {
                if(logoutTimers[p.ID]!=null)
                {
                    logoutTimers[p.ID].Stop();
                    logoutTimers[p.ID].Tick -= logoutTimer_Tick;
                    logoutTimers.Remove(p.ID);
                }
                logoutTimers.Remove(p.ID);
            }
            catch
            {
                Log.WriteDebug("Could not remove Timer");
            }

            // Remove from saved list
            for (int i = 0; i < LoggedInUsers.Count; i++)
                if (LoggedInUsers[i].Contains(p.ID.ToString()))
                {
                    foundIndex = i;
                    break;
                }
            if (foundIndex<0)
            {
                Log.WriteDebug("No valid user Timer to remove found");
                return;
            }
            LoggedInUsers.RemoveAt(foundIndex);
            Bierstrichler.Properties.Settings.Default.Save();
                
            Log.WriteInformation(App.CurrentVendor.Name + " removed " + p.Name + " from Present List.");
        }

        private ICommand depositCommand;

        public ICommand DepositCommand
        {
            get
            {
                if (depositCommand == null)
                    depositCommand = new RelayCommand(param => Deposit_Command(param));
                return depositCommand;
            }
            set
            {
                removePersonFromPresentListCommand = value;
            }
        }

        private void Deposit_Command(object param)
        {
            if(!(param is PersonViewModel))
                return;
            PersonViewModel p = (PersonViewModel)param as PersonViewModel;
            if (!(p.Model is Consumer))
                return;

            p.DepositMoney();
        }

        private ICommand specialPaymentCommand;

        public ICommand SpecialPaymentCommand
        {
            get
            {
                if (specialPaymentCommand == null)
                    specialPaymentCommand = new RelayCommand(param => SpecialPayment_Command(param));
                return specialPaymentCommand;
            }
            set
            {
                specialPaymentCommand = value;
            }
        }

        private void SpecialPayment_Command(object param)
        {
            if (!(param is PersonViewModel))
                return;
            PersonViewModel p = (PersonViewModel)param as PersonViewModel;
            if (!(p.Model is Consumer))
                return;

            p.SpecialPayment();
        }

        private ICommand payoutCommand;

        public ICommand PayoutCommand
        {
            get
            {
                if (payoutCommand == null)
                    payoutCommand = new RelayCommand(param => Payout_Command(param));
                return payoutCommand;
            }
            set
            {
                payoutCommand = value;
            }
        }

        private void Payout_Command(object param)
        {
            if (!(param is PersonViewModel))
                return;
            PersonViewModel p = (PersonViewModel)param as PersonViewModel;
            if (!(p.Model is Consumer))
                return;

            p.PayoutMoney();
        }

        #endregion Commands

        public bool IsCashier
        {
            get { return App.CurrentVendor.Rights.Cashier; }
        }

        private PersonViewModel selected;

        public PersonViewModel Selected
        {
            get { return selected; }
            private set
            {
                selected = value;
                RaisePropertyChanged();
            }
        }
    }
}
