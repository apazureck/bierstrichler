using Bierstrichler.Data.Enums;
using Bierstrichler.Data.Persons;
using Bierstrichler.Data.Persons.Korpos;
using Bierstrichler.ViewModels;
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

namespace Bierstrichler
{
    /// <summary>
    /// Interaction logic for StartupScreen.xaml
    /// </summary>
    public partial class LoginBox : Window
    {
        public LoginBox()
        {
            InitializeComponent();
        }

        public LoginBox(string UserName) : this()
        {
            NameBox.Text = UserName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "")
                NameBox.Focus();
            else
                PasswordBox.Focus();
        }

        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (PasswordBox.Password == "")
                {
                    PasswordBox.Focus();
                    return;
                }
                CheckPassword();
            }
        }

        private void CheckPassword()
        {
            wrongLoginMsg.Visibility = Visibility.Collapsed;
            wrongRightsMsg.Visibility = Visibility.Collapsed;
            try
            {
                if(App.Persons.Count<1)
                {
                    FoundUser = App.CurrentVendor;
                    Close();
                    return;
                }

                IList<Person> pl = new List<Person>();
                foreach (Person x in App.Persons)
                {
                    if(x.Name.Trim() == NameBox.Text.Trim() || x.Initials == NameBox.Text.Trim().ToUpper())
                    {
                        pl.Add(x);
                        continue;
                    }
                    if(x is Korpo && !string.IsNullOrEmpty(((Korpo)x).Vulgo) && ((Korpo)x).Vulgo == NameBox.Text.Trim())
                    {
                        pl.Add(x);
                        continue;
                    }
                }
                    

                FoundUser = null;

                foreach(Person p in pl)
                    if(CompareHashes(GetHash(PasswordBox), p.Password))
                    {
                        if (!(p.Rights.Worker || p.Rights.Moderator || p.Rights.Cashier || p.Rights.BeerAdmin || p.Rights.Administrator))
                        {
                            wrongRightsMsg.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            FoundUser = p;
                            Close();
                            return;
                        }
                    }
                throw new Exception();

            }
            catch (Exception e)
            {
                wrongLoginMsg.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;
            for(int i = 0; i<hash1.Length; i++)
                if (hash1[i] != hash2[i])
                    return false;
            return true;
        }

        private static byte[] GetHash(PasswordBox pwb)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pwb.Password.ToString());
            byte[] hash = md5.ComputeHash(inputBytes);
            return hash;
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                CheckPassword();
        }

        private void Button_Click_OK(object sender, RoutedEventArgs e)
        {
            CheckPassword();
        }

        public Person FoundUser { get; set; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //CheckPassword();
            if (FoundUser == null)
                DialogResult = false;
            else
                DialogResult = true;
            base.OnClosing(e);
        }
    }
}
