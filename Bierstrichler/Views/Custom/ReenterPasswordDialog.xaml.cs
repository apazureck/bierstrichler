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

namespace Bierstrichler.Views.Persons
{
    /// <summary>
    /// Interaction logic for ReenterPasswordDialog.xaml
    /// </summary>
    public partial class ReenterPasswordDialog : Window
    {
        public ReenterPasswordDialog()
        {
            InitializeComponent();
            PassWordBox.Focus();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            PasswordHash = GetHash(PassWordBox);
            DialogResult = true;
            this.Close();
        }

        public byte[] PasswordHash { get; set; }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            PasswordBox pwb = sender as PasswordBox;
            if (pwb == null)
                throw new ArgumentException("Sender is not a PasswordBox.", "sender");

            Button_OK_Click(this, null);
        }

        private static byte[] GetHash(PasswordBox pwb)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pwb.Password.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);
            return hash;
        }
    }
}
