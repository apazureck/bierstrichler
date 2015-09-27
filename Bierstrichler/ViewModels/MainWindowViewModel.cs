using Bierstrichler.Commands;
using Bierstrichler.Data.Events;
using Bierstrichler.Data.Persons;
using Bierstrichler.Events;
using Bierstrichler.ViewModels.Items;
using Bierstrichler.ViewModels.Persons;
using Bierstrichler.ViewModels.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bierstrichler.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public PersonListViewModel AllPersons { get; set; }
        public ItemListViewModel AllItems { get; set; }

        public DashListViewModel DashList { get; set; }

        public AppSettingsViewModel AppSettings { get; set; }

        public Questions.QuestionListViewModel Questions { get; set; }

        public MainWindowViewModel()
        {
            MainWindow mw = new MainWindow();
            Setup(mw);
            mw.Show();
        }

        public MainWindowViewModel(MainWindow view)
        {
            Setup(view);
        }

        private void Setup(MainWindow view)
        {
            view.Closed += mw_Closed;
            selectedTab = (TabItem)view.tabCtrl.Items[0];
            view.DataContext = this;
            StatusBar = new StatusBarViewModel(view.StatusBar);
            AllPersons = new PersonListViewModel(App.Persons);
            AllItems = new ItemListViewModel(App.Items);
            AppSettings = new AppSettingsViewModel();
            App.CurrentVendorChanged += App_CurrentVendorChanged;
            RaisePropertyChangedForAll();
        }

        void App_CurrentVendorChanged(object sender, PersonEventArgs e)
        {
            RaisePropertyChangedForAll();
        }

        void mw_Closed(object sender, EventArgs e)
        {
            MainWindow mw = (MainWindow)sender;
            Properties.Settings.Default.ApplicationHeight = mw.Height;
            Properties.Settings.Default.ApplicationPosX = mw.Left;
            Properties.Settings.Default.ApplicationPosY = mw.Top;
            Properties.Settings.Default.ApplicationWidth = mw.Width;
        }

        public StatusBarViewModel StatusBar { get; private set; }

        public double Top
        {
            get { return Properties.Settings.Default.ApplicationPosY; }
            set
            {
                Properties.Settings.Default.ApplicationPosY = value;
                RaisePropertyChanged();
            }
        }

        private TabItem selectedTab;

        public TabItem SelectedTab
        {
            get { return selectedTab; }
            set
            {
                switch (value.Name)
                {
                    case "tiDrinks":
                    case "tiUserAdmin":
                    case "tiProgramAdmin":
                    case "tiUserInformation":
                    case "tiStatistics":
                        LoginBox lb = new LoginBox(CurrentVendor.Name);
                        if (lb.ShowDialog() == true)
                            if ((lb.FoundUser == CurrentVendor))
                            {
                                selectedTab = value; 
                                RaisePropertyChanged();
                            }
                        break;
                    default:
                        selectedTab = value;
                        RaisePropertyChanged();
                        break;
                }
            }
        }

        public double Left
        {
            get { return Properties.Settings.Default.ApplicationPosX; }
            set
            {
                Properties.Settings.Default.ApplicationPosX = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get { return Properties.Settings.Default.ApplicationWidth; }
            set
            {
                Properties.Settings.Default.ApplicationWidth = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get { return Properties.Settings.Default.ApplicationHeight; }
            set
            {
                Properties.Settings.Default.ApplicationHeight = value;
                RaisePropertyChanged();
            }
        }
        
        Person CurrentVendor { 
            get 
            { 
                return App.CurrentVendor; 
            }
        }

        public PersonViewModel CurrentVendorViewModel
        {
            get { return new PersonViewModel(CurrentVendor); }
        }

        public bool IsAdministrator
        {
            get { return CurrentVendor.Rights.Administrator; }
            set
            {
                CurrentVendor.Rights.Administrator = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWorker
        {
            get { return CurrentVendor.Rights.Worker; }
            set
            {
                CurrentVendor.Rights.Worker = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCashier
        {
            get { return CurrentVendor.Rights.Cashier; }
            set
            {
                CurrentVendor.Rights.Cashier = value;
                RaisePropertyChanged();
            }
        }

        public bool IsModerator
        {
            get { return CurrentVendor.Rights.Moderator; }
            set
            {
                CurrentVendor.Rights.Moderator = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBeerAdmin
        {
            get { return CurrentVendor.Rights.BeerAdmin; }
            set
            {
                CurrentVendor.Rights.BeerAdmin = value;
                RaisePropertyChanged();
            }
        }

        internal void CreateDashList(Functional.DashBoard DashBoard)
        {
            DashList = new DashListViewModel(DashBoard, AllItems);
        }

        internal void CreateQuestionList(IList<Data.Questions.Question> questions)
        {
            Questions = new QuestionListViewModel(questions);
        }
    }
}
