using Bierstrichler.Commands;
using Bierstrichler.Data;
using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Items;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Persons.Korpos;
using Bierstrichler.Functional;
using Bierstrichler.Views.CashFlow;
using Bierstrichler.Views.Custom;
using Bierstrichler.Views.Persons;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Bierstrichler.ViewModels.Persons
{
    public class PersonViewModel : ViewModelBase
    {
        public event PersonChangedEventHandler PersonClassHasChanged;
        public Person Model { get; private set; }

        #region Properties
        /// <summary>
        /// Voller Name der Person (Vorname + Nachname)
        /// </summary>
        public string Name
        {
            get { return Model.Name; }
            set
            {
                RaisePropertyChanged();
            }
        }

        public string Vorname
        {
            get { return Model.Vorname; }
            set
            {
                Model.Vorname = value;
                RaisePropertyChanged();
            }
        }

        public string Nachname
        {
            get { return Model.Nachname; }
            set
            {
                Model.Nachname = value;
                RaisePropertyChanged();
            }
        }

        public MailAddress Email
        {
            get { return Model.Email == null ? null : new MailAddress(Model.Email); }
            set
            {
                try
                {
                    Model.Email = value.Address;
                }
                catch
                {
                    Model.Email = "";
                }
                RaisePropertyChanged();
            }
        }

        public Address Adresse
        {
            get { return Model.Adresse; }
            set
            {
                Model.Adresse = value;
                RaisePropertyChanged();
            }
        }

        public Gender Geschlecht
        {
            get { return Model.Geschlecht; }
            set
            {
                Model.Geschlecht = value;
                RaisePropertyChanged();
            }
        }

        public LogoutBehavior LogoutBehavior
        {
            get { return Model is Consumer ? ((Consumer)Model).LogoutBehavior : LogoutBehavior.Never; }
            set
            {
                if(Model is Consumer)
                {
                    ((Consumer)Model).LogoutBehavior = value;
                    RaisePropertyChanged();
                }
            }
        }
        

        public byte[] Password
        {
            get
            {
                return Model.Password;
            }
            set
            {
                if (Model.Password != null && value.SequenceEqual(Model.Password))
                    return;
                ReenterPasswordDialog dlg = new ReenterPasswordDialog();
                if (dlg.ShowDialog() == true)
                {
                    if (value.SequenceEqual(dlg.PasswordHash))
                    {
                        Model.Password = value;
                        MessageBox.Show("Passwort erfolgreich geändert.", "Passwort geändert", MessageBoxButton.OK, MessageBoxImage.Information);
                        RaisePropertyChanged();
                    }
                    else
                        MessageBox.Show("Passwort falsch eingegeben, bitte erneut eingeben.", "Passwort nicht geändert", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public ImageSource Image
        {
            get
            {
                if (image == null)
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

        public string Category { get { return UserType.ToString(); } }

        public UserType UserType
        {
            get
            {
                if (Model is Special)
                    return Data.Enums.UserType.Special;
                if (Model is Cartellbruder)
                    return Data.Enums.UserType.Cartellbruder;
                if (Model is Bundesbruder)
                    return Data.Enums.UserType.Bundesbruder;
                if (Model is Korpo)
                    return Data.Enums.UserType.Korpo;
                if (Model is Gast)
                    return Data.Enums.UserType.Gast;
                throw new ArgumentException("Konnte Usertype nicht bestimmen", "UserType");
            }
            set
            {
                try
                {
                    //App.Persons.Remove(Model);
                    switch (value)
                    {
                        case Data.Enums.UserType.Bundesbruder:
                            if (Model is Bundesbruder)
                                return;
                            else
                                Model = new Bundesbruder(Model);
                            break;
                        case Data.Enums.UserType.Korpo:
                            if (Model is Korpo)
                                return;
                            else
                                Model = new Korpo(Model);
                            break;
                        case Data.Enums.UserType.Gast:
                            if (Model is Gast)
                                return;
                            else
                                Model = new Gast(Model);
                            break;
                        case Data.Enums.UserType.Cartellbruder:
                            if (Model is Cartellbruder)
                                return;
                            else
                                Model = new Cartellbruder(Model);
                            break;
                        case Data.Enums.UserType.Special:
                            if (Model is Special)
                                return;
                            else
                                Model = new Special(Model);
                            break;
                        default:
                            throw new ArgumentException("Ungültiger Wert für das Enum", "UserType");
                    }
                    if (PersonClassHasChanged != null)
                        PersonClassHasChanged(this, new PersonEventArgs(Model));
                    RaisePropertyChangedForAll();
                }
                catch { }
            }
        }

        public bool RequirePasswordWhenAdding
        {
            get { return Model is Consumer ? ((Consumer)Model).Gesperrt : false; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).Gesperrt = value;
                    RaisePropertyChanged();
                }

            }
        }

        #region MoneyProperties

        public bool PaysBuingPrice
        {
            get { return Model is Consumer ? ((Consumer)Model).PaysBuingPrice : false; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).PaysBuingPrice = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool SkipNegativeSurcharge
        {
            get { return Model is Consumer ? ((Consumer)Model).SkipNegativeSurcharge : false; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).SkipNegativeSurcharge = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Guthaben
        {
            get
            {
                if (Model is Consumer)
                    return ((Consumer)Model).Guthaben;
                else
                    return 0;
            }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).Guthaben = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Rank
        {
            get
            {
                if (Model is Consumer)
                    return BeerCharts.GetRank((Consumer)this.Model);
                else
                    throw new Exception("Person is no Comsumer");
            }
        }

        public int TotalDrinks
        {
            get { return Model is Consumer ? ((Consumer)Model).TotalDrinks : -1; }
        }

        public int TotalChartDrinks
        {
            get { return Model is Consumer ? ((Consumer)Model).TotalChartDrinks : 0; }
        }

        public int Advance
        {
            get { return IsListedInBeerCharts ? BeerCharts.GetAdvance(this.Model as Consumer) : 0; }
        }

        public int Deficite
        {
            get { return IsListedInBeerCharts ? BeerCharts.GetDeficite(this.Model as Consumer) : 0; }
        }

        public ObservableCollection<Consumed> History
        {
            get { return new ObservableCollection<Consumed>(((Consumer)Model).History); }
        }

        public ObservableCollection<Consumed> RecentDrinks
        {
            get
            {
                return new ObservableCollection<Consumed>(recentDrinks);
            }
        }
        List<Consumed> recentDrinks = new List<Consumed>();

        public bool IsListedInBeerCharts
        {
            get { return Model is Consumer ? ((Consumer)Model).ListedInCharts : false; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).ListedInCharts = value;
                    RaisePropertyChanged();
                }

            }
        }

        public decimal MinGuthaben
        {
            get { return Model is Consumer ? ((Consumer)Model).MinGuthaben : decimal.MinValue; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).MinGuthaben = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ThresholdBehavior ThresholdBehavior
        {
            get { return Model is Consumer ? ((Consumer)Model).ThresholdBehavior : Data.Enums.ThresholdBehavior.None; }
            set
            {
                if (Model is Consumer)
                {
                    ((Consumer)Model).ThresholdBehavior = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Rights

        public bool IsGuest
        {
            get { return Model.Rights.Guest; }
            set
            {
                Model.Rights.Guest = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWorker
        {
            get { return Model.Rights.Worker; }
            set
            {
                Model.Rights.Worker = value;
                RaisePropertyChanged();
            }
        }

        public bool IsCashier
        {
            get { return Model.Rights.Cashier; }
            set
            {
                Model.Rights.Cashier = value;
                RaisePropertyChanged();
            }
        }

        public bool IsModerator
        {
            get { return Model.Rights.Moderator; }
            set
            {
                Model.Rights.Moderator = value;
                RaisePropertyChanged();
            }
        }

        public bool IsBeerAdmin
        {
            get { return Model.Rights.BeerAdmin; }
            set
            {
                Model.Rights.BeerAdmin = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAdministrator
        {
            get { return Model.Rights.Administrator; }
            set
            {
                Model.Rights.Administrator = value;
                RaisePropertyChanged();
            }
        }

        public bool IsKorpo
        {
            get { return Model is Korpo; }
            set
            {
                RaisePropertyChanged();
            }
        }

        public bool IsBundesbruder
        {
            get { return Model is Bundesbruder; }
            set
            {
                RaisePropertyChanged();
            }
        }

        #endregion Rights

        #region KorpoProperties
        public string Vulgo
        {
            get { return Model is Korpo ? ((Korpo)Model).Vulgo : ""; }
            set
            {
                if(Model is Korpo)
                {
                    ((Korpo)Model).Vulgo = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Status Status
        {
            get { return Model is Korpo ? ((Korpo)Model).Status : Data.Enums.Status.Unknown; }
            set
            {
                if (Model is Korpo)
                {
                    ((Korpo)Model).Status = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("StatusString");
                }
            }
        }

        public string StatusString { get { return Status.ToString(); } }

        public DateTime? Rezeptionsdatum
        {
            get { return Model is Korpo ? (DateTime?)((Korpo)Model).Rezeptionsdatum : null; }
            set
            {
                if (Model is Korpo)
                {
                    ((Korpo)Model).Rezeptionsdatum = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion KorpoProperties

        #region CurrentVendor

        public bool IsChangeFinanceAllowed
        {
            get { return App.CurrentVendor.Rights.BeerAdmin; }
        }

        public bool IsCurrentVendorAdmin
        {
            get
            {
                return App.CurrentVendor.Rights.Administrator;
            }
        }

        public bool IsCurrentVendorModerator
        {
            get
            {
                return App.CurrentVendor.Rights.Moderator;
            }
        }

        #endregion CurrentVendor

        #endregion Properties

        public PersonViewModel(Person Model)
        {
            this.Model = Model;
            BeerCharts.ChartsUpdated += BeerCharts_ChartsUpdated;
            App.CurrentVendorChanged += App_CurrentVendorChanged;
        }

        #region Evenhandlers

        void App_CurrentVendorChanged(object sender, PersonEventArgs e)
        {
            RaisePropertyChanged("IsCurrentVendorAdmin");
            RaisePropertyChanged("IsChangeFinanceAllowed");
        }

        void BeerCharts_ChartsUpdated(object sender, EventArgs e)
        {
            RaisePropertyChanged("Rank");
        }

        #endregion Evenhandlers

        #region Commands

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
            OpenFileDialog dlg = DialogFactory.GetNewImageFileDialog();

            // Process open file dialog box results 
            if (dlg.ShowDialog() == true)
            {
                // Add Image Uri to Model.
                Model.RelativeImagePath = App.AddToPersonImages(dlg.FileName);
                CreateNewImageSource();
                // Raise property changed to update the image.
                RaisePropertyChanged("Image");
            }
        }

        private ICommand takePictureCommand;

        public ICommand TakePictureCommand
        {
            get
            {
                if (takePictureCommand == null)
                    takePictureCommand = new RelayCommand(param => TakePicture_Command(param));
                return takePictureCommand;
            }
            set
            {
                takePictureCommand = value;
            }
        }

        private void TakePicture_Command(object param)
        {
            CameraWindowDialog dlg = new CameraWindowDialog();
            if(dlg.ShowDialog() == true)
            {
                Model.RelativeImagePath = App.AddToPersonImages(dlg.tempPath);
                BitmapImage bi = new BitmapImage(new Uri(Model.RelativeImagePath, UriKind.Absolute));
                Model.ImageCutOut = new Rectangle(20, 20, bi.PixelWidth-40, bi.PixelHeight-40);
                CreateNewImageSource();
                RaisePropertyChanged("Image");
            }
        }

        private ImageSource CreateNewImageSource()
        {
            if (Model.RelativeImagePath != null)
            {
                CroppedBitmap cb;
                try
                {
                    cb = new CroppedBitmap(new BitmapImage(Model.ImageUri), new Int32Rect(Model.ImageCutOut.X, Model.ImageCutOut.Y, Model.ImageCutOut.Width, Model.ImageCutOut.Height));
                    return cb;
                }
                catch (Exception)
                {
                    try
                    {
                        string lpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "images", "persons", Path.GetFileName(Model.RelativeImagePath));
                        cb = new CroppedBitmap(
                            new BitmapImage(new Uri(lpath, UriKind.Absolute)),
                            new Int32Rect(Model.ImageCutOut.X, Model.ImageCutOut.Y, Model.ImageCutOut.Width, Model.ImageCutOut.Height));
                        Model.RelativeImagePath = lpath;
                    }
                    catch (Exception)
                    {
                        Model.RelativeImagePath = null;
                        cb = new CroppedBitmap();
                    }
                    App.SavePersonsListAsync();
                    return cb;
                }
            }
            else
                return new BitmapImage();

        }

        private ICommand modifyPictureCommand;

        public ICommand ModifyPictureCommand
        {
            get
            {
                if (modifyPictureCommand == null)
                    modifyPictureCommand = new RelayCommand(param => ModifyPicture_Command(param));
                return modifyPictureCommand;
            }
            set
            {
                modifyPictureCommand = value;
            }
        }

        private void ModifyPicture_Command(object param)
        {
            if (Model.RelativeImagePath == null)
            {
                MessageBox.Show("Kein Bild ausgewählt.", "Fehler beim Öffnen des Bildes", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ChangeImageDialog dlg = new ChangeImageDialog(new BitmapImage(Model.ImageUri), Model.ImageCutOut);
            if (dlg.ShowDialog() == true)
            {
                Model.ImageCutOut = dlg.ImageCutOut;
                CreateNewImageSource();
                RaisePropertyChanged("Image");
            }
        }

        private ICommand revertChangeCommand;

        public ICommand RevertChangeCommand
        {
            get
            {
                if (revertChangeCommand == null)
                    revertChangeCommand = new RelayCommand(param => RevertChange_Command(param));
                return revertChangeCommand;
            }
            set
            {
                revertChangeCommand = value;
            }
        }

        private void RevertChange_Command(object param)
        {
            Consumed c = param as Consumed;
            if (c == null)
                return;
            if(c.Type == ConsumeType.Purchase)
            {
                Item itm = App.Items.Find(x => x == c.Item);
                itm.RemoveHistoryChange(c);
                if (((Consumer)Model).PaysBuingPrice)
                    this.Guthaben += itm.PriceBuying;
                else
                    this.Guthaben += itm.PriceSelling;
            }
            RemoveFromHistory(c);

        }

        private void RemoveFromHistory(Consumed c)
        {
            // Remove Consumed from List
            ((Consumer)Model).RemoveHistoryChange(c);
            RaisePropertyChanged("History");
            App.SaveItemListAsync();
            App.SavePersonsListAsync();
            Log.WriteWarning(App.CurrentVendor.Name + " removed one Item from " + Model.Name);
        }

        #endregion Commands

        #region Methods

        public void DepositMoney()
        {
            if(!(Model is Consumer))
                throw new ArgumentException("Model is not a consumer", "Model");
            PayInWindow piw = new PayInWindow();
            
            if(piw.ShowDialog() == true)
            {
                if (!(Model is Consumer))
                    throw new ArgumentException("Model is not a consumer", "Model");
                Consumer c = (Consumer)Model;
                if (piw.Betrag <= 0)
                    MessageBox.Show(piw.Betrag.ToString("C", CultureInfo.CurrentCulture) + " ist kein gültiger Betrag.", "Fehler beim Einzahlen", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    c.DepositMoney(piw.Betrag, App.CurrentVendor);
                    RaisePropertyChanged("Guthaben");
                    Log.WriteInformation(App.CurrentVendor.Name + " initiated a payin for " + Name + " over " + piw.Betrag.ToString() + " Euro.");
                }
            }
        }

        public void Buy(Item item)
        {
            if (!(Model is Consumer))
                throw new ArgumentException("Model is not a consumer", "Model");
            Consumer c = (Consumer)Model;
            decimal oldCash = Guthaben;
            c.Pay(item, (c.Guthaben >= 0 || SkipNegativeSurcharge ? 1 : Properties.Settings.Default.NegativeDepositFactor), App.CurrentVendor);
            CheckThresholdBehavior(oldCash);
            RaisePropertyChanged("Guthaben");
            RaisePropertyChanged("TotalDrinks");
            BeerCharts.UpdateChartsAsync();
            App.SaveItemListAsync();
            App.SavePersonsListAsync();
        }

        override protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //if(!propertyName.Equals("Rank"))
            //    Log.WriteInformation(App.CurrentVendor.Name + " changed " + propertyName + " of " + Name + " to " + this.GetType().GetProperty(propertyName).GetValue(this).ToString());
            base.RaisePropertyChanged(propertyName);
            if(propertyName=="Guthaben")
                RaisePropertyChanged("RecentDrinks");
            if (propertyName == "RecentDrinks")
                return;
            App.SavePersonsListAsync();
        }

        public override string ToString()
        {
            return "PersonViewModel for " + Model.ToString();
        }

        internal void SpecialPayment()
        {
            if (!(Model is Consumer))
                throw new ArgumentException("Model is not a consumer", "Model");
            SpecialPaymentWindow piw = new SpecialPaymentWindow();
            piw.Title = "Sonderzahlung";
            if (piw.ShowDialog() == true)
            {
                if (!(Model is Consumer))
                    throw new ArgumentException("Model is not a consumer", "Model");
                if (piw.Betrag <= 0)
                    MessageBox.Show(piw.Betrag.ToString("C", CultureInfo.CurrentCulture) + " ist kein gültiger Betrag.", "Fehler bei der Sonderzahlung", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    Consumer c = (Consumer)Model;
                    decimal oldCash = Guthaben;
                    c.SpecialPayment(piw.Betrag, App.CurrentVendor, piw.CommentText);
                    CheckThresholdBehavior(oldCash);
                    RaisePropertyChanged("Guthaben");
                    Log.WriteInformation(App.CurrentVendor.Name + " initiated a Special Payment \"" + piw.CommentText + "\" for " + Name + " over " + piw.Betrag.ToString() + " Euro.");
                }
            }
        }

        private void CheckThresholdBehavior(decimal oldCash)
        {
            switch(ThresholdBehavior)
            {
                case Data.Enums.ThresholdBehavior.SendEmail:
                    if (oldCash - MinGuthaben >= 0 && Guthaben - MinGuthaben < 0)
                        MailScheduler.SendReminderToUserAsync(Model);
                    break;
                default:
                    break;
            }
        }

        internal void PayoutMoney()
        {
            if (!(Model is Consumer))
                throw new ArgumentException("Model is not a consumer", "Model");
            PayInWindow piw = new PayInWindow();

            if (piw.ShowDialog() == true)
            {
                if (!(Model is Consumer))
                    throw new ArgumentException("Model is not a consumer", "Model");
                if (piw.Betrag <= 0)
                    MessageBox.Show(piw.Betrag.ToString("C", CultureInfo.CurrentCulture) + " ist kein gültiger Betrag.", "Fehler beim Auszahlen", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    Consumer c = (Consumer)Model;
                    c.PayoutMoney(piw.Betrag, App.CurrentVendor);
                    RaisePropertyChanged("Guthaben");
                    Log.WriteInformation(App.CurrentVendor.Name + " initiated a payout for " + Name + " over " + piw.Betrag.ToString() + " Euro.");
                }
            }
        }

        ReaderWriterLock taskLock = new ReaderWriterLock(){};
        async void MakeRecentlyBoughtList()
        {
            if (taskLock.IsWriterLockHeld)
                return;
            try
            {
                taskLock.AcquireWriterLock(10);
                await Task.Run(() =>
                {
                    try
                    {
                        int end = Math.Min(4, ((Consumer)Model).History.Count+1);
                        recentDrinks.Clear();
                        for (int i = 1; i < end; i++)
                            recentDrinks.Add(((Consumer)Model).History[((Consumer)Model).History.Count - i]);
                        RaisePropertyChanged("RecentDrinks");
                    }
                    catch { }
                });
            }
            finally
            {
                taskLock.ReleaseWriterLock();
            }
        }

        #endregion Methods

        #region RefreshListUpdate

        DispatcherTimer recentDrinkListTimer;

        internal void StopRefreshRecentDrinkListUpdateTimer()
        {
            recentDrinkListTimer.Stop();
        }

        void recentDrinkListTimer_Tick(object sender, EventArgs e)
        {
            MakeRecentlyBoughtList();
        }

        internal void StartRefreshRecendDrinkListUpdateTimer()
        {
            MakeRecentlyBoughtList();
            if (recentDrinkListTimer == null)
            {
                recentDrinkListTimer = new DispatcherTimer(DispatcherPriority.Background);
                recentDrinkListTimer.Interval = new TimeSpan(0, 0, 1);
                recentDrinkListTimer.Tick += recentDrinkListTimer_Tick;
            }
            recentDrinkListTimer.Start();
        }

        #endregion RefreshListUpdate
    }
}
