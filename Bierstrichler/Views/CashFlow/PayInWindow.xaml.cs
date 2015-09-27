using Bierstrichler.Data.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bierstrichler.Views.CashFlow
{
    /// <summary>
    /// Interaction logic for PayInWindow.xaml
    /// </summary>
    public partial class PayInWindow : Window
    {
        public PayInWindow()
        {
            InitializeComponent();
            betrag.Focus();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public decimal Betrag { get; set; }

        public string Text { get; set; }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordHash = GetHash(PassWordBox);
                if (App.CurrentVendor.Password == null)
                {
                    MessageBox.Show("Kein gültiger Account ausgewählt, bitte melden Sie sich an.", "Anmeldungsfehler");
                    this.Close();
                }
                byte[] pwd = App.CurrentVendor.Password;

                if (!PasswordHash.SequenceEqual(pwd))
                {
                    MessageBox.Show(App.CurrentVendor.Vorname + ", Du hast Dein Passwort falsch eingegeben. Dadurch kann die Einzahlung nicht vorgenommen werden.", "Passwort falsch", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DialogResult = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show(App.CurrentVendor.Vorname + ", Du hast Dein Passwort falsch eingegeben. Dadurch kann die Einzahlung nicht vorgenommen werden.", "Passwort falsch", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public byte[] PasswordHash { get; set; }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            PasswordBox pwb = sender as PasswordBox;
            if (pwb == null)
                throw new ArgumentException("Sender is not a PasswordBox.", "sender");
            byte[] hash = GetHash(pwb);

            PasswordHash = hash;

            Button_OK_Click(null, null);
        }

        private static byte[] GetHash(PasswordBox pwb)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pwb.Password.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);
            return hash;
        }

        private void betrag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                PassWordBox.Focus();
        }
    }
}
