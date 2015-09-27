using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Events;
using Bierstrichler.Data.Global;
using Bierstrichler.Data.Items;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Persons.Korpos;
using Bierstrichler.Data.Questions;
using Bierstrichler.Data.Serialization;
using Bierstrichler.Events;
using Bierstrichler.Functional;
using Bierstrichler.ViewModels;
using Bierstrichler.ViewModels.Items;
using Bierstrichler.ViewModels.Persons;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace Bierstrichler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static event PersonChangedEventHandler CurrentVendorChanged;

        public static string ItemsFile { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "items.dat"); } }
        public static string PersonsFile { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler","data","persons.dat"); } }
        static string questionsFile { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", "questions.xml"); } }
        static string imageFiles { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "images"); } }
        static string personImages { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "images", "persons"); } }
        static string itemImages { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bierstrichler", "data", "images", "items"); } }
        static string emailTemplates { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", "email"); } }
        static string userFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", "counting"); } }

        public static ItemList Items { get; set; }
        public static PersonList Persons { get; set; }
        public static List<Question> Questions { get; set; }

        internal static StatusBarViewModel StatusBar;

        public static DashBoard DashBoard { get; set; }

        public static MainWindowViewModel MainWindowViewModel { get; set; }

        #region Properties

        /// <summary>
        /// Person, die aktuell strichelt und somit die Verantwortung übernimmt.
        /// </summary>
        public static Person CurrentVendor 
        {
            get  { return currentVendor; }
            set
            {
                if(currentVendor!=null)
                    Log.WriteInformation("Current Vendor changed from " + currentVendor.Name + " to " + value.Name);
                currentVendor = value;
                StatusBar.CurrentUser = currentVendor.Name;
                if (CurrentVendorChanged != null)
                    CurrentVendorChanged(null, new PersonEventArgs(value));
            }
        }
        private static Person currentVendor;

        #endregion Properties

        public static string AddToItemImages(string fileName)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            string newLocation = Path.Combine(itemImages, photoID.ToString() + ".png");  //file name 

            encoder.Frames.Add(BitmapFrame.Create(new BitmapImage(new Uri(fileName, UriKind.Absolute))));

            using (var filestream = new FileStream(newLocation, FileMode.Create))
                encoder.Save(filestream);
            return newLocation;
        }

        public static string AddToPersonImages(string fileName)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            string newLocation = Path.Combine(personImages, photoID.ToString() + ".png");  //file name 

            encoder.Frames.Add(BitmapFrame.Create(new BitmapImage(new Uri(fileName, UriKind.Absolute))));

            using (var filestream = new FileStream(newLocation, FileMode.Create))
                encoder.Save(filestream);
            return newLocation;
        }

        internal static string AddToPersonImages(System.Windows.Media.Imaging.BitmapSource imageSource)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            Guid photoID = System.Guid.NewGuid();
            string newLocation = Path.Combine(personImages, photoID.ToString() + ".jpg");

            encoder.Frames.Add(BitmapFrame.Create(imageSource));

            using (var filestream = new FileStream(newLocation, FileMode.Create))
                encoder.Save(filestream);
            return newLocation;
            
        }

        /// <summary>
        /// Saves the Item List to the items file.
        /// </summary>
        public static void SaveItemListAsync()
        {
            StatusBar.SetProgressBar("Saving Items", 0.0);
            Items.SaveItemListAsync();
            StatusBar.Progress = 100.0;
            StatusBar.ResetProgressBar(1000);
        }

        internal static void LoadFiles()
        {
            CreateDataFiles();

            try
            {
                StatusBar.SetProgressBar("Opening Log", 0.0);
                // Starte Log
                Log.OpenLog();
                Log.WriteInformation("Bierstrichler started.");

                StatusBar.SetProgressBar("Loading Persons", 60.0);

                Persons = PersonList.LoadFromFile(PersonsFile, Path.Combine(Bierstrichler.Properties.Settings.Default.PersonListSecureCopy, "persons.dat"));

                StatusBar.SetProgressBar("Loading Finished", 100.0);
                StatusBar.ResetProgressBar(1000);

                // Lade Items aus gespeicherter Liste
                StatusBar.SetProgressBar("Loading Items", 30.0);

                Items = ItemList.LoadFromFile(ItemsFile, Path.Combine(Bierstrichler.Properties.Settings.Default.ItemListSecureCopy, "items.dat"));

                BeerCharts.Items = Items;
                BeerCharts.Persons = Persons;

                Task.Run(() =>
                {
                    try
                    {
                        Log.WriteInformation("Opening Question List.");
                        using (FileStream fs = new FileStream(questionsFile, FileMode.Open))
                        {
                            XmlSerializer bf = new XmlSerializer(typeof(List<Question>));
                            Questions = (List<Question>)bf.Deserialize(fs);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteWarningFromException("Could not load Question List.", ex);
                        Questions = new List<Question>();
                    }
                });
            }
            catch (FileLoadException fle)
            {
                MessageBox.Show("Eine Datei für den Bierstrichler konnte nicht korrekt geladen werden. Stellen Sie sicher, dass der Bierstrichler nicht zwei mal geöffnet wurde.");
                Application.Current.Shutdown();
            }
            
        }

        /// <summary>
        /// Erstellt die Datafiles u. nötige Dateistruktur
        /// </summary>
        private static void CreateDataFiles()
        {
            // User/App/Roaming/Bierstrichler Subfolders erstellen
            foreach (string directory in Bierstrichler.Properties.Settings.Default.ApplicationDirectories)
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Bierstrichler",directory)))
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Bierstrichler",directory));

            if (!File.Exists(ItemsFile))
                File.Create(ItemsFile);

            if(!File.Exists(PersonsFile))
                File.Create(PersonsFile);

            //MyDocuments Ordnerstruktur erstellen
            foreach (string directory in Bierstrichler.Properties.Settings.Default.UserDirectories)
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", directory)))
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Bierstrichler", directory));

            // Kopiert die deployten Sachen in MyDocuments/Bierstrichler
            foreach(string s in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_deploy")))
                if(!File.Exists(Path.Combine(emailTemplates, Path.GetFileName(s))))
                    File.Copy(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "_deploy",s), Path.Combine(emailTemplates, Path.GetFileName(s)), false);

            if (!File.Exists(questionsFile))
                File.Create(questionsFile);
        }

        internal static void StartApplicationMainWindow()
        {
            MainWindow mw = new MainWindow();
            MainWindowViewModel = new MainWindowViewModel(mw);
            StatusBar = MainWindowViewModel.StatusBar;
            StatusBar.CurrentUser = CurrentVendor.Name;
            DashBoard = new DashBoard(Persons);
            MainWindowViewModel.CreateDashList(DashBoard);
            MainWindowViewModel.CreateQuestionList(Questions);
            BeerCharts.UpdateCharts();
            QuestionMaster.Reset();
            mw.Show();
            App.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Bierstrichler.Properties.Settings.Default.Save();
            Persons.SavePersonsList();
            Items.SaveItemList();
            ClearUnusedImages();
            Log.WriteInformation("Bierstrichler closed.");
        }

        private void ClearUnusedImages()
        {
            //TODO: Methode um alle nicht benutzten bilder zu löschen.
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
            Log.LogLevel = Bierstrichler.Properties.Settings.Default.LogLevel;
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(
                CultureInfo.CurrentCulture.IetfLanguageTag))
            );
            StatusBar = new StatusBarViewModel(new Views.StatusBar());
            if (Bierstrichler.Properties.Settings.Default.LoggedInUsers == null)
                Bierstrichler.Properties.Settings.Default.LoggedInUsers = new System.Collections.Specialized.StringCollection();

            LoadFiles();
            MailScheduler.Setup();
            if(!UpdateIfNeccessary())
                ShowLogin();
        }
        private void ShowLogin()
        {
            if (App.Persons.Count < 1)
            {
                App.CurrentVendor = new Gast()
                {
                    Nachname = "Administrator",
                    Rights = new Rights()
                    {
                        Administrator = true,
                        BeerAdmin = true,
                        Cashier = true,
                        Guest = true,
                        Moderator = true,
                        Worker = true
                    },
                    ID = new Guid()
                };
                StartApplicationMainWindow();
            } 
            else if (Bierstrichler.Properties.Settings.Default.AutoLoginActive)
            {
                try
                {
                    Person p = Persons.Find(x => x.ID.Equals(Bierstrichler.Properties.Settings.Default.StandardUser));
                    if (p == null)
                        throw new Exception();
                    App.CurrentVendor = p;
                }
                catch
                {
                    App.CurrentVendor = new Gast()
                    {
                        Nachname = "Fuchs",
                        Vorname = "Fräulein",
                        Rights = new Rights()
                        {
                            Administrator = false,
                            BeerAdmin = false,
                            Cashier = false,
                            Guest = false,
                            Moderator = false,
                            Worker = false
                        },
                        ID = new Guid()
                    };
                }
                StartApplicationMainWindow();
            }
            else
            {
                LoginBox lb = new LoginBox();
                if (lb.ShowDialog() == true)
                {
                    CurrentVendor = lb.FoundUser;
                    StartApplicationMainWindow();
                }
                else
                    App.Current.Shutdown();
            }
        }
        internal static void SavePersonsListAsync()
        {
            StatusBar.SetProgressBar("Saving Persons", 0.0);
            Persons.SavePersonsListAsync();
            StatusBar.Progress = 100.0;
            StatusBar.ResetProgressBar(1000);
        }
        /// <summary>
        /// Exports Data to a user specified file using a file dialog.
        /// </summary>
        /// <returns>true if successfully exported</returns>
        public static bool ExportData()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "xml";
            sfd.Filter = "XML-File(*.xml)|*.xml";

            if(sfd.ShowDialog()==true)
            {
                string path = sfd.FileName;
                try
                {
                    ExportData(path);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private static void ExportData(string path)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ListContainer));
                    ListContainer lc = new ListContainer(Persons, Items);
                    xml.Serialize(tw, lc);
                }
                Log.WriteInformation("Data successfully exported to " + path);
            }
            catch (Exception e)
            {
                Log.WriteErrorFromException("Could not export Data due to an error.", e);
                throw;
            }
        }        

        public static bool ImportData()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "xml";
            ofd.Filter = "XML-File(*.xml)|*.xml";
            if(ofd.ShowDialog()==true)
            {
                string p = ofd.FileName;
                ImportData(p);
                return true;
            }
            return false;
        }

        private static void ImportData(string p)
        {
            using (TextReader tr = new StreamReader(p))
            {
                XmlSerializer xml = new XmlSerializer(typeof(ListContainer));
                ListContainer tmp = (ListContainer)xml.Deserialize(tr);

                Persons.Clear();
                tmp.Persons.ForEach(x => Persons.Add(x));
                Items.Clear();
                tmp.Items.ForEach(x => Items.Add(x));
                Persons.SavePersonsList();
                Items.SaveItemList();
                Log.WriteInformation("Data successfully imported from " + p);
            }
        }

        internal static void DeleteAllHistoryData()
        {
            Persons.ClearHistory();
            Items.ClearHistory();
            SavePersonsListAsync();
            SaveItemListAsync();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if !DEBUG
            string errmsg = Log.WriteErrorFromException("Application Crash", e.Exception) + "\n\nCurrent User: \n";
            //MessageBox.Show(errmsg, "Die Anwendung hat eine Ausnahme versursacht.", MessageBoxButton.OK, MessageBoxImage.Error);
            MailScheduler.SendMailNow("andreas@pazureck.de", "Bierstrichler Bugreport " + DateTime.Now.ToString(), errmsg);
            e.Handled = true;
            try
            {
                if (App.Current.ShutdownMode == System.Windows.ShutdownMode.OnExplicitShutdown)
                    App.Current.Shutdown();
            }
            catch { }
#endif
        }

        static ReaderWriterLock rwLock = new ReaderWriterLock();
        internal static void SaveQuestionsAsync()
        {
            Task.Run(() =>
                {
                    StatusBar.SetProgressBar("Saving Questions", 0.0);
                    try
                    {
                        rwLock.AcquireWriterLock(-1);
                        for (int i = 0; i < 3; i++)
                            try
                            {
                                using (FileStream fs = new FileStream(questionsFile, FileMode.Truncate))
                                {
                                    XmlSerializer xmls = new XmlSerializer(typeof(List<Question>));
                                    if (Questions.Count > 0)
                                        xmls.Serialize(fs, Questions);
                                }
                            }
                            catch (Exception e)
                            {
                                if (i == 2)
                                    MessageBox.Show("Stellen Sie sicher, dass der Dateipfad vorhanden ist.", "questions.dat konnte nicht gespeichert werden.\n\nUrsache:" + e.Message);
                                Thread.Sleep(1000);
                            }
                    }
                    finally
                    {
                        rwLock.ReleaseLock();
                    }
                    StatusBar.Progress = 100.0;
                    StatusBar.ResetProgressBar(1000);
                });
        }
        private static bool UpdateIfNeccessary()
        {
            bool didUpdate = false;
            UpdateCheckInfo info = null;
            if(!string.IsNullOrEmpty(Bierstrichler.Properties.Settings.Default.UpdateExport))
            {
                ImportData(Bierstrichler.Properties.Settings.Default.UpdateExport);
                Bierstrichler.Properties.Settings.Default.UpdateExport = "";
                try
                {
                    File.Delete(Bierstrichler.Properties.Settings.Default.UpdateExport);
                }
                catch(Exception e)
                {
                    Log.WriteErrorFromException("Could not delete temp file from export.", e);
                }
            }

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();
                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return false;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return false;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return false;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        MessageBoxResult dr = MessageBox.Show("An update is available. Would you like to update the application now?", "Update Available", MessageBoxButton.YesNo);
                        if (!(dr == MessageBoxResult.Yes))
                            doUpdate = false;
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        MessageBox.Show("This application has detected a mandatory update from your current " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will now install the update and restart.",
                            "Update Available", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            string p = Path.GetTempFileName();
                            ExportData(p);
                            Bierstrichler.Properties.Settings.Default.UpdateExport = p;
                            Bierstrichler.Properties.Settings.Default.Save();
                            ad.Update();
                            MessageBox.Show("The application has been upgraded, and will now restart.");
                            didUpdate = true;
                            System.Windows.Forms.Application.Restart();
                            Application.Current.Shutdown();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                            return false;
                        }
                    }
                }
            }
            return didUpdate;
        }
    }
}
