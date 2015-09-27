using Bierstrichler.Data.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Bierstrichler.Data.Global
{
    public static class Log
    {
        public static event LogEntryEvent MessageLogged;

        private static StreamWriter logFile;
        public static string LogFilePath { get; private set; }
        public static void OpenLog(string fileName = null, string path = null)
        {
            if (logFile != null)
                return;
            // Erstellt den Pfad
            path = path == null ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Bierstrichler\Log" : path;
            fileName = fileName == null ? CreateFileName() : fileName;
            string fullpath = Path.Combine(path, fileName);
            // Erstellt den Ordner, falls noch nicht vorhanden
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            // Erstellt die Datei, falls noch keine vorhanden und
            // Öffnet das Logfile.

            if (File.Exists(fullpath))
            {
                try
                {
                    logFile = new StreamWriter(fullpath, true);
                    Log.WriteLine("Application Startup", LogLevel.Information);
                }
                catch (IOException)
                {
                    // Falls der Prozess schon läuft fragen ob man ihn killen soll.
                    Process[] parr = Process.GetProcessesByName("Bierstrichler");
                    if (parr.Length > 0)
                        if(MessageBox.Show("Der Bierstrichler ist schon geöffnet. Soll er beendet werden?", "Bierstrichler läuft schon", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            parr[0].Kill();
                            Thread.Sleep(1000);
                            try
                            {
                                logFile = new StreamWriter(fullpath, true);
                                Log.WriteLine("Application Startup", LogLevel.Information);
                            }
                            catch(IOException)
                            {
                                MessageBox.Show("Der Bierstrichler kann das Logfile nicht öffnen. Es wurde keine weitere Instanz des Bierstrichlers gefunden. Stellen Sie sicher, dass die Logdatei nicht mit einem Texteditor geöffnet ist. Der Bierstrichler wird jetzt beendet.", "Logfile kann nicht geöffnet werden.", MessageBoxButton.OK);
                                Application.Current.Shutdown();
                            }
                        }
                        else
                            Application.Current.Shutdown();
                    else
                    {
                        MessageBox.Show("Der Bierstrichler kann das Logfile nicht öffnen. Es wurde keine weitere Instanz des Bierstrichlers gefunden. Stellen Sie sicher, dass die Logdatei nicht mit einem Texteditor geöffnet ist. Der Bierstrichler wird jetzt beendet.", "Logfile kann nicht geöffnet werden.", MessageBoxButton.OK);
                        Application.Current.Shutdown();
                    }
                }
            }
                
            else
                logFile = new StreamWriter(File.Create(fullpath));
            
            if (logFile != null)
                LogFilePath = fullpath;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        public static LogLevel LogLevel { get; set; }
        private static string CreateFileName()
        {
            return DateTime.Today.ToString("yy-MM-dd") + ".log";
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CloseLog();
        }
        /// <summary>
        /// Schreibt eine Zeile ins Log.
        /// </summary>
        /// <param name="message">Nachricht</param>
        /// <param name="logLevel">Debug, Information, Warning, Error</param>
        private static void WriteLine(string message, LogLevel logLevel)
        {
            Debug.WriteLine(message);
            if(logLevel <= LogLevel)
            {
                LogEntry le = new LogEntry(logLevel, message, DateTime.Now);
                message = le.ToString();
                WriteToLogFile(message);
                if (MessageLogged != null)
                    MessageLogged(null, new LogEventArgs(le));
            }
        }
        /// <summary>
        /// Schreibt die Nachricht ins Logfile, falls der StreamWriter aktiv ist.
        /// </summary>
        /// <param name="message">Nachricht, die ins Logfile geschrieben werden soll.</param>
        private static void WriteToLogFile(string message)
        {
            if(logFile != null)
                    Task.Run(() => {
                        lock(logFile)
                            logFile.WriteLine(message);
                    });
        }
        
        /// <summary>
        /// Schreibt eine Errormessage.
        /// </summary>
        /// <param name="message">Nachricht, die geschrieben werden soll</param>
        public static void WriteError(string message)
        {
            Log.WriteLine(message, LogLevel.Error);
        }
        /// <summary>
        /// Schreibt eine Warning.
        /// </summary>
        /// <param name="message">Nachricht, die geschrieben werden soll</param>
        public static void WriteWarning(string message)
        {
            Log.WriteLine(message, LogLevel.Warning);
        }
        /// <summary>
        /// Schreibt eine Informationsmessage.
        /// </summary>
        /// <param name="message">Nachricht, die geschrieben werden soll</param>
        public static void WriteInformation(string message)
        {
            Log.WriteLine(message, LogLevel.Information);
        }
        /// <summary>
        /// Schreibt eine Debugnachricht.
        /// </summary>
        /// <param name="message">Nachricht, die geschrieben werden soll</param>
        public static void WriteDebug(string message)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase callingMethod = stackTrace.GetFrame(1).GetMethod();
            string type = callingMethod.DeclaringType.Name;
            string method = callingMethod.Name;
            Log.WriteLine("Caller: " + type + "." + method + ": " + message, LogLevel.Debug);
        }

        public static void CloseLog()
        {
            if (logFile != null)
                logFile.Close();
        }
        /// <summary>
        /// Returns
        /// </summary>
        /// <returns></returns>
        public static string[] ShowLogFiles()
        {
            if(LogFilePath == null)
                throw new FileLoadException("No log file directory specified. Call OpenLog method first.");
            return Directory.GetFiles(Path.GetDirectoryName(LogFilePath), "*.log");
        }

        public static List<LogEntry> ReadEntries(string path)
        {
            List<LogEntry> retList = new List<LogEntry>();
            using(StreamReader sr = new StreamReader(path))
            {
                while(sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    retList.Add(new LogEntry(line));
                }
            }
            return retList;
        }
        /// <summary>
        /// Erstellt eine Warning aus einer Exception mit allen InnerExceptions
        /// </summary>
        /// <param name="message">Nachricht</param>
        /// <param name="exception">Exception, die ausgegeben werden soll</param>
        public static void WriteWarningFromException(string message, Exception exception)
        {
            string exMessage = " Reason: ";
            MakeRecursiveMessageFromException(exception, ref exMessage);
            WriteWarning(message + exMessage);
        }

        /// <summary>
        /// Erstellt eine Errormessage aus einer Exception mit allen InnerExceptions
        /// </summary>
        /// <param name="message">Nachricht</param>
        /// <param name="exception">Exception, die ausgegeben werden soll</param>
        public static string WriteErrorFromException(string message, Exception exception)
        {
            string exMessage = " Reason: ";
            MakeRecursiveMessageFromException(exception, ref exMessage);
            WriteError(message + exMessage);
            return exMessage;
        }

        public static void MakeRecursiveMessageFromException(Exception exception, ref string message)
        {
            message += exception.Message;
            if (exception.InnerException != null)
            {
                message += " : ";
                MakeRecursiveMessageFromException(exception.InnerException, ref message);
            }
                
        }

        public static void DebugException(string p, Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
