using Bierstrichler.Data.Persons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bierstrichler.Functional.ExtensionMethods;
using System.Diagnostics;
using System.Net;
using Bierstrichler.Data.Global;
using System.Xml.Serialization;
using System.Timers;
using System.Windows;
using Bierstrichler.Data.Items;
using System.Windows.Threading;
using System.Net.Mime;

namespace Bierstrichler.Functional
{
    public static class MailScheduler
    {
        private static object sendLock = new object();
        private static DispatcherTimer sendTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
        private static Queue<MailMessage> SendList = new Queue<MailMessage>();

        public static List<HistoryMail> MailHistory { get; set; }

        private static string HostAddress { get { return Properties.Settings.Default.MailServer.Split(':')[0].Trim(); } }
        private static int Port { get { return Convert.ToInt32(Properties.Settings.Default.MailServer.Split(':')[1].Trim()); } }
        private static string DisplayName { get { return Properties.Settings.Default.MailDisplayName; } }
        private static string MailAddress { get { return Properties.Settings.Default.MailAddress; } }
        private static string UserName { get { return Properties.Settings.Default.MailDisplayName; } }
        private static SecureString Password { get { return SecureStringSerializer.DecryptString(Properties.Settings.Default.MailPassword); } }
        private static int MaxMailsPerCall { get { return Properties.Settings.Default.MailMaxMailsPerCall; } }
        static string MailFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"Bierstrichler", "email"); } }

        static DateTime NextInvoice
        {
            get { return Properties.Settings.Default.NextInvoice; }
            set
            {
                Properties.Settings.Default.NextInvoice = value;
                Properties.Settings.Default.Save();
            }
        }

        static DateTime LastInvoice
        {
            get { return Properties.Settings.Default.LastInvoice; }
            set
            {
                Properties.Settings.Default.LastInvoice = value;
                Properties.Settings.Default.Save();
            }
        }

        static Bierstrichler.Data.Enums.InvoicePeriod InvoiceSchedule { get { return Properties.Settings.Default.InvoicePeriod; } }

        public static void Setup()
        {
            if(LastInvoice == default(DateTime))
            {
                LastInvoice = DateTime.Now;
                NextInvoice = SetNextInvoiceDate(DateTime.Now);
            }
            MailHistory = new List<HistoryMail>();
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            sendTimer.Tick += sendTimer_Elapsed;
            lock (sendLock)
            {
                Task.Run(() =>
                {
                    LoadHistoryFromHistoryFile();
                });
            }
            if (NextInvoice == default(DateTime))
                NextInvoice = DateTime.Now;
            sendTimer.Interval = Properties.Settings.Default.MailSendInterval;
            sendTimer.Start();
        }

        /// <summary>
        /// Gets called by the sendtimer, but can also be called by user.
        /// </summary>
        /// <param name="sender">ignored</param>
        /// <param name="e">ignored</param>
        private static void sendTimer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                if (InvoicesOnSchedule(DateTime.Now))
                {
                    Log.WriteInformation("Invoices are due. Putting on the send mail schedule.");
                    SendInvoices(App.Persons, LastInvoice);
                    NextInvoice = SetNextInvoiceDate(DateTime.Now);
                    Log.WriteInformation("Next invoice is due on " + NextInvoice.ToShortDateString());
                }
                else
                    Log.WriteDebug("Next invoice is due on " + NextInvoice.ToShortDateString());
                SendScheduledMails();
            }
            catch (Exception ex)
            {
                Log.WriteErrorFromException("Could not send scheduled Mails", ex);
            }
        }

        private static void SendInvoices(PersonList personList, DateTime invoiceDate)
        {
            Log.WriteDebug("Sending Invoices.");
            foreach (Consumer c in personList)
                if (c.Email != null)
                {
                    Log.WriteDebug("Sending Invoice to " + c.Name);
                    SendInvoiceAsync(c, invoiceDate);
                }
            LastInvoice = DateTime.Now;
        }

        private static DateTime SetNextInvoiceDate(DateTime last)
        {
            switch (InvoiceSchedule)
            {
                case Data.Enums.InvoicePeriod.Annual:
                    return last.AddYears(1);
                case Data.Enums.InvoicePeriod.Monthly:
                    return last.AddMonths(1);
                case Data.Enums.InvoicePeriod.Quarterly:
                    return last.AddMonths(3);
                case Data.Enums.InvoicePeriod.Semester:
                    return last.AddMonths(6);
                case Data.Enums.InvoicePeriod.Trimester:
                    return last.AddMonths(4);
                case Data.Enums.InvoicePeriod.Weekly:
                    return last.AddDays(7);
                default:
                    return last.AddMonths(1);
            }
        }

        public static void SendInvoice(Consumer c, DateTime invoiceDate)
        {
            MailMessage m = CreateInvoiceMail(c, invoiceDate);
            SendMailNow(m);
        }

        /// <summary>
        /// Creates the invoice mail to the Consumer c using the invoiceDate.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="invoiceDate"></param>
        /// <returns>null if the user did not have any consumed items</returns>
        private static MailMessage CreateInvoiceMail(Consumer c, DateTime invoiceDate)
        {
            MailMessage m = new MailMessage();
            m.From = new MailAddress(MailAddress);
            m.To.Add(new MailAddress(c.Email));
            m.Subject = "Deine Bierrechnung vom " + DateTime.Now.ToString("dd. M. yyyy");
            m.IsBodyHtml = true;
            List<Data.Items.Consumed> conList = c.History.FindAll(x => x.Date > invoiceDate);
            if (conList.Count > 0)
            {
                Log.WriteDebug("Sending invoice to " + c.Email + "with " + conList.Count + " entries.");
                m.Body = SetValuesInPlaceHolders(File.ReadAllText(MailFolder + @"\standardInvoice.html", Encoding.Default), c, invoiceDate, conList);
            }
            else
            {
                Log.WriteDebug("Nothing to send to " + c.Email + ".");
                m = null;
            }
            return m;
        }

        public static void SendInvoiceAsync(Consumer c, DateTime invoiceDate)
        {
            MailMessage m = CreateInvoiceMail(c, invoiceDate);
            if (m != null)
                SendMailScheduled(m);
        }

        private static Regex regToken = new Regex(@"\{\$([a-zA-Z_][^\$]*)\$\}", RegexOptions.Compiled, new TimeSpan(0, 0, 10));
        private static Regex regIf = new Regex(@"\{\$IF ([a-zA-Z_][^\$]*)\$\}(.*)\{\$ELSE\$\}(.*)\{\$ENDIF\$\}", RegexOptions.Compiled, new TimeSpan(0, 0, 10));
        private static Regex regComparer = new Regex(@"(.*[^<=>])([<=>]+)(.*)", RegexOptions.Compiled, new TimeSpan(0, 0, 10));
        private static Regex regRegion = new Regex(@"\{\$BEGIN (?<tag>[a-zA-Z_][^\$]*)\$\}(?<inner>.*)\{\$END \1\$\}", RegexOptions.Singleline, new TimeSpan(0, 0, 10));
        private static Regex regImageUrl = new Regex("(?<start><img.*src=\\\")(?<url>[^\\\"]*)(?<end>[^/]*/>)", RegexOptions.Singleline, new TimeSpan(0, 0, 10));

        /// <summary>
        /// Replaces the values in the string with the information by the person using a "$Token|Format$" Syntax. The Tokens are the Property names of the person.
        /// </summary>
        /// <param name="s">strings where the tokens should be replaced.</param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string SetValuesInPlaceHolders(string s, Person p, Dictionary<string, object> additionalValues = null)
        {
            try
            {
                s = regIf.Replace(s, delegate(Match m)
                {
                    string compare = m.Groups[1].Value;
                    Match cm = regComparer.Match(compare);
                    string left = cm.Groups[1].Value;
                    dynamic leftValue = p.GetType().GetProperty(left).GetValue(p);
                    string right = cm.Groups[3].Value;
                    dynamic rightValue = CastOtherValue(right, leftValue);
                    string comparer = cm.Groups[2].Value;
                    string replacement = "";
                    switch (comparer)
                    {
                        case "<":
                            replacement = leftValue < rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        case ">":
                            replacement = leftValue > rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        case "<=":
                            replacement = leftValue <= rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        case ">=":
                            replacement = leftValue >= rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        case "=":
                            replacement = leftValue == rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        case "<>":
                            replacement = leftValue != rightValue ? m.Groups[2].Value : m.Groups[3].Value;
                            break;
                        default:
                            throw new InvalidOperationException("Not a valid comparer");
                    }
                    return SetValuesInPlaceHolders(replacement, p, additionalValues);
                });

                s = regToken.Replace(s, delegate(Match m)
                {
                    string[] values = m.Groups[1].Value.Split('|');
                    object repValue;
                    switch (values[0])
                    {
                        case "CurrentDate":
                            repValue = DateTime.Now;
                            break;
                        default:
                            try
                            {
                                repValue = p.GetType().GetProperty(values[0]).GetValue(p);
                            }
                            catch
                            {
                                if (additionalValues == null)
                                    return m.Value;
                                repValue = additionalValues[values[0]];
                            }
                            break;
                    }
                    return FormatValue(values, repValue);
                });
            }
            catch (Exception e)
            {
                if (DebugMode)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Konnte Token nicht finden.");
                    sb.AppendLine("Erlaubte Token:");
                    foreach (PropertyInfo pi in p.GetType().GetProperties())
                        sb.AppendLine(pi.Name);
                    MessageBox.Show(sb.ToString(), "Fehler beim Versenden der Test Email.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                }
                else
                    Log.WriteErrorFromException("The Method MailScheduler.SetValuesInPlaceholders threw an exception.", e);
            }
            return s;
        }

        private static object CastOtherValue(string right, IComparable leftValue)
        {
            return Convert.ChangeType(right, leftValue.GetType());
        }

        private static string FormatValue(string[] values, object repValue)
        {
            if (values.Length > 1)
                return string.Format("{0:" + values[1] + "}", repValue);
            else
                return repValue.ToString();
        }

        private static string SetValuesInPlaceHolders(string s, Consumer c, DateTime invoiceDate, List<Data.Items.Consumed> conList)
        {
            s = regRegion.Replace(s, delegate(Match m)
            {
                try
                {
                    if (m.Groups["tag"].Value.Equals("ConsumedItems"))
                    {
                        string historyPart = m.Groups["inner"].Value;
                        return AddHistoryToMail(historyPart.Trim(), conList);
                    }
                    return "";
                }
                catch (Exception e)
                {
                    return "MAKE HISTORY FAILED!";
                }
            });
            return SetValuesInPlaceHolders(s, c);
        }

        private static bool InvoicesOnSchedule(DateTime dateToCheck)
        {
            return dateToCheck >= NextInvoice;
        }

        static private void SendScheduledMails()
        {
            Task.Run(() =>
            {
                lock (sendLock)
                {
                    int i = 0;
                    int max = Math.Min(MaxMailsPerCall, SendList.Count);
                    Log.WriteInformation("MailScheduler: Sending " + max.ToString() + " scheduled mails.");
                    MailMessage m = null;
                    while (i < max)
                    {
                        try
                        {
                            m = SendList.Dequeue();
                            SendMailNow(m);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteErrorFromException("MailScheduler: Could not send mail " + (i + 1).ToString() + "/" + max.ToString() + " to " + m.To[0].Address, ex);
                        }
                        finally
                        {
                            i++;
                        }
                    }
                    Log.WriteInformation("MailScheduler: Sending complete.");
                }
            });
        }

        /// <summary>
        /// Catcht den Event, damit die noch ausstehenden Mails abgearbeitet werden können. Es werden einfach alle Mails rausgeballert.
        /// TODO: Eventuell Mails auf Festplatte speichern und beim nächsten Start verschicken.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            App.StatusBar.ProgressBarText = "Sending Mails";
            App.StatusBar.Progress = 0;
            int i = 0;
            int max = SendList.Count;
            while (SendList.Count > 0)
            {
                i++;
                App.StatusBar.UpdateProgress(i, max);
                try
                {
                    SendMailNow(SendList.Dequeue());
                }
                catch (Exception ex)
                {

                    Log.WriteErrorFromException("Error sending Mails at Shutdown", ex);
                }
            }
        }

        private static void LoadHistoryFromHistoryFile()
        {
            //string line;
            //using (StreamReader sr = new StreamReader(Path.Combine(MailFolder, "history.txt")))
            //{
            //    while((line = sr.ReadLine()) != null)
            //    {
            //        try
            //        {
            //            if(!String.IsNullOrEmpty(line))
            //                MailHistory.Add(new HistoryMail(line));
            //        }
            //        catch
            //        {

            //        }
            //    }
            //}
        }

        public static void SendReminderToUserAsync(Person p)
        {
            Task.Run(() =>
                {
                    lock (sendLock)
                    {
                        SendReminderToUser(p);
                    }
                });
        }

        public static void SendReminderToUser(Person p)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(MailAddress);
            mail.To.Add(new MailAddress(p.Email));
            mail.Subject = "Reminder: Unterschreitung deines Mindestguthabens";
            mail.Body = SetValuesInPlaceHolders(File.ReadAllText(MailFolder + @"\standardReminder.txt", Encoding.Default), p);
            // Can set to false, if you are sending pure text.

            SendMailNow(mail);
        }

        private static void SendMailNow(MailMessage mail)
        {
            using (SmtpClient smtp = new SmtpClient(HostAddress, Port))
            {
                smtp.Credentials = new NetworkCredential(MailAddress, Password);
                smtp.EnableSsl = true;
                smtp.SendCompleted += ssc_SendCompleted;
                try
                {
                    if (mail.IsBodyHtml)
                        AttatchImageToMail(mail);
                    smtp.Send(mail);
                    MailHistory.Add(new HistoryMail(mail));
                    for (int i = 0; i < mail.To.Count; i++)
                        Log.WriteInformation("Mailing to " + mail.To[i].ToString());
                    SaveHistory(MailHistory.Last());
                }
                catch (Exception ex)
                {
                    Log.WriteErrorFromException("Fehler beim Senden der Email: ", ex);
                    if (DebugMode)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(ex.Message);
                        if (ex.InnerException != null)
                            sb.AppendLine(ex.InnerException.Message);
                        MessageBox.Show(sb.ToString(), "Fehler beim Versenden der Test Email.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
                    }
                }
            }
        }

        /// <summary>
        /// Attaches an image to a mail.
        /// </summary>
        /// <param name="mail">Mail the images defined in the mail body as html tag should be replaced</param>
        private static void AttatchImageToMail(MailMessage mail)
        {
            List<LinkedResource> resList = new List<LinkedResource>();
            mail.Body = regImageUrl.Replace(mail.Body, delegate(Match m)
            {
                try
                {
                    string filePath = m.Groups["url"].Value;
                    LinkedResource inline = new LinkedResource(Path.Combine(MailFolder, filePath));
                    inline.ContentId = Guid.NewGuid().ToString();
                    Attachment att = new Attachment(Path.Combine(MailFolder, filePath));
                    att.ContentDisposition.Inline = true;
                    mail.Attachments.Add(att);
                    resList.Add(inline);
                    Log.WriteDebug("Replacing " + m.Groups["url"] + " with " + inline.ContentId);
                    return m.Groups["start"] + "cid:" + inline.ContentId + m.Groups["end"];
                }
                catch (Exception ex)
                {
                    Log.DebugException("MailScheduler.AttachImageToMail", ex);
                    return m.Value;
                }
            });
            Log.WriteDebug("Adding " + resList.Count.ToString() + " attachments to the Email.");
            if (resList.Count > 0)
            {
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(mail.Body, null, MediaTypeNames.Text.Html);
                foreach (LinkedResource lr in resList)
                    avHtml.LinkedResources.Add(lr);
                mail.AlternateViews.Add(avHtml);
            }
        }

        /// <summary>
        /// Adds a mail to the "to-send-list"
        /// </summary>
        /// <param name="mail"></param>
        public static void SendMailScheduled(MailMessage mail)
        {
            if (mail == null)
                throw new ArgumentNullException("mail");
            SendList.Enqueue(mail);
            Log.WriteInformation("Added Mail \"" + mail.Subject + "\" to \"" + mail.To[0].ToString() + "\" to MailScheduler.");
        }

        private static void SaveHistory(HistoryMail historyMail)
        {
            //Task.Run(() =>
            //{
            //    lock (MailFolder)
            //    {
            //        File.AppendAllLines(Path.Combine(MailFolder, "history.txt"), new string[] { historyMail.ToString() });
            //    }
            //});
        }

        internal static void SendMailNow(string mailAddress, string Subject, string errmsg)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(MailAddress);
            mail.To.Add(new MailAddress(mailAddress));
            mail.Subject = Subject;
            mail.Body = errmsg;

            SendMailNow(mail);
        }

        private static void ssc_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Log.WriteError("Send Mail Error: " + e.Error + " UserState: " + e.UserState);
                Debug.WriteLine(e.Error + " UserState: " + e.UserState);
            }
            else
                Log.WriteInformation("Email successfully sent.");
        }

        private static string AddHistoryToMail(string s, List<Data.Items.Consumed> conList)
        {
            Dictionary<Item, int> differentItems = new Dictionary<Item, int>();
            foreach (Data.Items.Consumed con in conList)
                if (differentItems.ContainsKey(con.Item))
                    differentItems[con.Item]++;
                else if (!con.Item.Equals(Guid.Empty))
                    differentItems.Add(con.Item, 1);
            StringBuilder sb = new StringBuilder();
            char[] copy = s.ToCharArray();

            foreach (KeyValuePair<Item, int> item in differentItems)
            {
                try
                {
                    string line = ReplaceItem(copy, item);
                    sb.AppendLine(line);
                }
                catch
                {

                }

            }
            return sb.ToString();
        }

        private static string ReplaceItem(char[] copy, KeyValuePair<Item, int> item)
        {
            string line = new String(copy);
            Dictionary<string, object> repVals = new Dictionary<string, object>(3);
            Item i = App.Items.Find(x => x.Equals(item.Key));
            repVals.Add("ItemName", i.Name);
            repVals.Add("TotalPrize", i.PriceSelling * item.Value);
            repVals.Add("NumOfConsumed", item.Value);
            return regToken.Replace(line, delegate(Match m)
            {
                string[] repArr = m.Groups[1].Value.Split('|');
                return FormatValue(repArr, repVals[repArr[0]]);
            });
        }

        internal static void SendTestMail(Person p)
        {
            DebugMode = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(MailAddress);
            mail.To.Add(new MailAddress(p.Email));
            mail.Subject = "Testmail";
            mail.Body = "Testmail";

            SendMailNow(mail);
            DebugMode = false;
        }

        /// <summary>
        /// Indicates or sets the MailScheduler to Debug Mode. This is used for extended user feedback for test mails.
        /// </summary>
        public static bool DebugMode { get; set; }

        internal static void ChangeMailSendInterval(TimeSpan value)
        {
            Log.WriteDebug("Stopping SendMailTimer.");
            sendTimer.Stop();
            sendTimer.Interval = value;
            sendTimer.Start();
            Log.WriteDebug("Starting SendMailTimer.");
        }

        internal static void ChangeNextInvoiceDate()
        {
            NextInvoice = SetNextInvoiceDate(LastInvoice);
        }
    }
}