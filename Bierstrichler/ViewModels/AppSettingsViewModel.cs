using Bierstrichler.Commands;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Questions;
using Bierstrichler.Functional;
using Bierstrichler.ViewModels.Persons;
using Bierstrichler.Views.Custom;
using Bierstrichler.Data.Enums;
using DataContractWrappers;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Windows.Threading;

namespace Bierstrichler.ViewModels
{
    public class AppSettingsViewModel : ViewModelBase
    {
        public AppSettingsViewModel()
        {
            PropertyChanged += AppSettingsViewModel_PropertyChanged;
            Model.SettingChanging += Model_SettingChanging;
            Log.MessageLogged += Log_MessageLogged;
        }

        Bierstrichler.Properties.Settings Model { get { return Properties.Settings.Default; } }

        #region Fields

        private object logListLock = new object();

        #endregion Fields

        #region Eventhandlers

        void Model_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer(DispatcherPriority.ApplicationIdle, App.Current.Dispatcher);
            dt.Interval = new TimeSpan(0, 0, 2);
            dt.Tick += delegate(object sender2, EventArgs e2)
                {
                    try
                    {
                        Log.WriteDebug("Model_SettingChanged Timer event occurred for Property " + e.SettingName);
                        dt.Stop();
                        RaisePropertyChanged(e.SettingName);
                    }
                    catch { }
                };
            dt.Start();
        }

        void Log_MessageLogged(object sender, Data.Events.LogEventArgs e)
        {
            lock(logListLock)
            {
                try
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LogEntries.Add(e.Entry);
                        if (LogEntries.Count > maxNumOfEntries)
                            LogEntries.RemoveAt(0);
                    }));
                    
                }
                catch
                { }
            }
        }

        void AppSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Task.Run(() =>
            {
                Model.Save();
            });
        }

        #endregion Eventhandlers

        #region Properties

        #region Support

        private int maxNumOfEntries = 50;
        public int MaxNumOfEntries
        {
            get { return maxNumOfEntries; }
            set
            {
                maxNumOfEntries = value;
                Log.WriteDebug("Changed Max entries to display on Log view to " + value.ToString() + ".");
                RaisePropertyChanged();
            }
        }

        private FastObservableCollection<LogEntry> logEntries = new FastObservableCollection<LogEntry>();

        public FastObservableCollection<LogEntry> LogEntries
        {
            get { return logEntries; }
            set
            {
                logEntries = value;
                RaisePropertyChanged();
            }
        }

        public PersonListViewModel AllPersons
        {
            get { return App.MainWindowViewModel.AllPersons; }
            set { App.MainWindowViewModel.AllPersons = value; RaisePropertyChanged(); }
        }

        #endregion Support

        #region Wrapped

        public LogLevel LogLevel
        {
            get { return Model.LogLevel; }
            set
            {
                Model.LogLevel = value;
                Log.LogLevel = value;
                Log.WriteInformation("Changed LogLevel to " + value.ToString());
                RaisePropertyChanged();
            }
        }

        public bool AutoLoginActive
        {
            get { return Model.AutoLoginActive; }
            set
            {
                Model.AutoLoginActive = value;
                Log.WriteInformation(App.CurrentVendor.Name + " changed Autologin to \"" + value.ToString() + "\"");
                RaisePropertyChanged();
            }
        }

        public decimal NegativeDepositFactor
        {
            get { return Model.NegativeDepositFactor; }
            set
            {
                Log.WriteInformation(App.CurrentVendor.Name + " changed the negative Deposit factor from " + NegativeDepositFactor.ToString() + " to " + value.ToString());
                Model.NegativeDepositFactor = value;
                RaisePropertyChanged();
            }
        }

        public PersonViewModel StandardUser
        {
            get
            {
                foreach (PersonViewModel pvm in AllPersons.Persons)
                    if (pvm.Model.Id.Equals(Model.StandardUser))
                        return pvm;
                return null;
            }
            set
            {
                Model.StandardUser = value.Model.Id;
                Log.WriteInformation(App.CurrentVendor.Name + " changed the Autologin User to " + value.Name);
                RaisePropertyChanged();
            }
        }

        #region Mail
        public string MailServer
        {
            get { return Model.MailServer; }
            set
            {
                Model.MailServer = value.Trim();
                RaisePropertyChanged();
            }
        }

        public string MailDisplayName
        {
            get { return Model.MailDisplayName; }
            set
            {
                Model.MailDisplayName = value.Trim();
                RaisePropertyChanged();
            }
        }

        public string MailAddress
        {
            get { return Model.MailAddress; }
            set
            {
                Model.MailAddress = value.Trim();
                RaisePropertyChanged();
            }
        }

        public string MailLoginName
        {
            get { return Model.MailLoginName; }
            set
            {
                Model.MailLoginName = value.Trim();
                RaisePropertyChanged();
            }
        }

        public SecureString MailPassword
        {
            get { return SecureStringSerializer.DecryptString(Model.MailPassword); }
            set
            {
                Model.MailPassword = SecureStringSerializer.EncryptString(value);
                RaisePropertyChanged();
            }
        }

        #endregion Mail

        public string ItemListSecureCopy
        {
            get { return Model.ItemListSecureCopy; }
            set
            {
                Model.ItemListSecureCopy = value;
                RaisePropertyChanged();
            }
        }

        public string PersonListSecureCopy
        {
            get { return Model.PersonListSecureCopy; }
            set
            {
                Model.PersonListSecureCopy = value;
                RaisePropertyChanged();
            }
        }

        public EncoderDevice WebCamSource
        {
            get { return EncoderDevices.FindDevices(EncoderDeviceType.Video).FirstOrDefault(x => x.Name == Model.WebCamSource); }
            set
            {
                Model.WebCamSource = value.Name;
                RaisePropertyChanged();
            }
        }

        public int WebCamSourceIndex
        {
            get 
            {
                try
                {
                    Collection<EncoderDevice> edl = EncoderDevices.FindDevices(EncoderDeviceType.Video);
                    EncoderDevice ed = edl.FirstOrDefault(x => x.Name == Model.WebCamSource);
                    return edl.IndexOf(ed);
                }
                catch { return -1; }
            }
            set
            {
                Collection<EncoderDevice> edl = EncoderDevices.FindDevices(EncoderDeviceType.Video);
                Model.WebCamSource = edl[value].Name;
                RaisePropertyChanged();
            }
        }

        #endregion Wrapped

        #endregion Properties

        #region Questiongame
        public bool QuestionGameActive
        {
            get { return Model.QuestionGameActive; }
            set
            {
                Model.QuestionGameActive = value;
                RaisePropertyChanged();
            }
        }

        public int QuestionMinClicks
        {
            get { return Model.QuestionMinClicks; }
            set
            {
                Model.QuestionMinClicks = value;
                RaisePropertyChanged();
            }
        }

        public int QuestionMaxClicks
        {
            get { return Model.QuestionMaxClicks; }
            set
            {
                Model.QuestionMaxClicks = value;
                RaisePropertyChanged();
            }
        }

        #endregion Questiongame

        #region Commands

        private ICommand exportDataCommand;

        public ICommand ExportDataCommand
        {
            get
            {
                if (exportDataCommand == null)
                    exportDataCommand = new RelayCommand(param => ExportData_Command(param));
                return exportDataCommand;
            }
            set
            {
                exportDataCommand = value;
            }
        }

        private void ExportData_Command(object param)
        {
            if(App.ExportData()==true)
                Log.WriteInformation(App.CurrentVendor.Name + " exported the data.");
        }

        private ICommand deleteHistoryCommand;

        public ICommand DeleteHistoryCommand
        {
            get
            {
                if (deleteHistoryCommand == null)
                    deleteHistoryCommand = new RelayCommand(param => DeleteHistory_Command(param));
                return deleteHistoryCommand;
            }
            set
            {
                deleteHistoryCommand = value;
            }
        }

        private void DeleteHistory_Command(object param)
        {
            if(MessageBox.Show("Wollen Sie wirklich die gesamte History löschen?", "Gesamte History löschen", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (App.ExportData() == true)
                {
                    App.DeleteAllHistoryData();
                    Log.WriteInformation(App.CurrentVendor.Name + " deleted the complete history.");
                }
                else
                    MessageBox.Show("Die History kann nur gelöscht werden, wenn sie vorher exportiert wurde.", "Fehler beim Löschen", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICommand importDataCommand;

        public ICommand ImportDataCommand
        {
            get
            {
                if (importDataCommand == null)
                    importDataCommand = new RelayCommand(param => ImportData_Command(param));
                return importDataCommand;
            }
            set
            {
                importDataCommand = value;
            }
        }

        private void ImportData_Command(object param)
        {
            if (App.ImportData() == true)
                Log.WriteInformation(App.CurrentVendor + " imported data.");
        }

        private ICommand sendTestMailCommand;

        public ICommand SendTestMailCommand
        {
            get
            {
                if (sendTestMailCommand == null)
                    sendTestMailCommand = new RelayCommand(param => SendTestMail_Command(param));
                return sendTestMailCommand;
            }
            set
            {
                sendTestMailCommand = value;
            }
        }

        private void SendTestMail_Command(object param)
        {
            if(param is PersonViewModel)
                MailScheduler.SendTestMail(((PersonViewModel)param).Model);
            else
                MessageBox.Show("Geben Sie einen Benutzernamen bei \"An Nutzer\" ein.", "Kein Nutzer ausgewählt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private ICommand sendTestInvoiceCommand;

        public ICommand SendTestInvoiceCommand
        {
            get
            {
                if (sendTestInvoiceCommand == null)
                    sendTestInvoiceCommand = new RelayCommand(param => SendTestInvoice_Command(param));
                return sendTestInvoiceCommand;
            }
            set
            {
                sendTestInvoiceCommand = value;
            }
        }

        public InvoicePeriod InvoicePeriod
        {
            get { return Model.InvoicePeriod; }
            set
            {
                Model.InvoicePeriod = value;
                MailScheduler.ChangeNextInvoiceDate();
                RaisePropertyChanged();
                RaisePropertyChanged("NextInvoice");
            }
        }

        public DateTime LastInvoice
        {
            get { return Model.LastInvoice; }
            set
            {
                Model.LastInvoice = value;
                RaisePropertyChanged();
            }
        }

        public DateTime NextInvoice
        {
            get { return Model.NextInvoice; }
            set
            {
                Model.NextInvoice = value;
                RaisePropertyChanged();
            }
        }


        private void SendTestInvoice_Command(object param)
        {
            if (param is PersonViewModel && ((PersonViewModel)param).Model is Consumer)
            {
                DateTime testInvoice = DateTime.Now;
                MailScheduler.DebugMode = true;
                MailScheduler.SendInvoice(((PersonViewModel)param).Model as Consumer, DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)));
                MailScheduler.DebugMode = false;
            }
            else
                MessageBox.Show("Geben Sie einen Benutzernamen bei \"An Nutzer\" ein.", "Kein Nutzer ausgewählt", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        public TimeSpan MailSendInterval
        {
            get { return Model.MailSendInterval; }
            set
            {
                Model.MailSendInterval = value;
                MailScheduler.ChangeMailSendInterval(value);
                Log.WriteInformation("MailSendInterval changed to " + value.ToString());
                RaisePropertyChanged();
            }
        }



        public int MailMaxMailsPerCall
        {
            get { return Model.MailMaxMailsPerCall; }
            set
            {
                Model.MailMaxMailsPerCall = value;
                Log.WriteInformation("Maximum Mails per call changed to " + value.ToString());
                RaisePropertyChanged();
            }
        }


        private ICommand changePasswordCommand;

        public ICommand ChangePasswordCommand
        {
            get
            {
                if (changePasswordCommand == null)
                    changePasswordCommand = new RelayCommand(param => ChangePassword_Command(param));
                return changePasswordCommand;
            }
            set
            {
                changePasswordCommand = value;
            }
        }

        private void ChangePassword_Command(object param)
        {
            if (param is PasswordBox)
            {
                MailPassword = ((PasswordBox)param).SecurePassword;
            }
        }

        private ICommand selectFolderCommand;

        public ICommand SelectFolderCommand
        {
            get
            {
                if (selectFolderCommand == null)
                    selectFolderCommand = new RelayCommand(param => SelectFolder_Command(param));
                return selectFolderCommand;
            }
            set
            {
                selectFolderCommand = value;
            }
        }

        private void SelectFolder_Command(object param)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if(dlg.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                ((System.Windows.Controls.TextBox)param).Text = dlg.SelectedPath;
            }
        }

        private ICommand importQuestionsCommand;

        public ICommand ImportQuestionsCommand
        {
            get
            {
                if (importQuestionsCommand == null)
                    importQuestionsCommand = new RelayCommand(param => ImportQuestions_Command(param));
                return importQuestionsCommand;
            }
            set
            {
                importQuestionsCommand = value;
            }
        }

        private void ImportQuestions_Command(object param)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    XmlSerializer bf = new XmlSerializer(typeof(List<Question>));
                    List<Question> qList = (List<Question>)bf.Deserialize(fs);
                    SelectQuestionsToImportDialog qtidlg = new SelectQuestionsToImportDialog(qList);
                    qtidlg.ShowDialog();
                    foreach (Question q in qtidlg.QuestionsToImport)
                    {
                        Question qtr = App.Questions.Find(x=>x.QuestionText.Equals(q.QuestionText));
                        if (qtr != null)
                            App.Questions.Remove(qtr);
                        App.Questions.Add(q);
                    }
                    App.SaveQuestionsAsync();
                }
            }
        }

        #endregion
    }
}
