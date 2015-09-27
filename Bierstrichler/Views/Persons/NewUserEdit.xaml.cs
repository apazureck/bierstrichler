using Bierstrichler.ViewModels.Persons;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bierstrichler.Views.Persons
{
    /// <summary>
    /// Interaction logic for UserEdit.xaml
    /// </summary>
    public partial class NewUserEdit : UserControl
    {
        public NewUserEdit()
        {
            InitializeComponent();
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            PasswordBox pwb = sender as PasswordBox;
            if (pwb == null)
                throw new ArgumentException("Sender is not a PasswordBox.", "sender");

            OpenReEnterPasswordDialog(pwb);
        }

        private void OpenReEnterPasswordDialog(PasswordBox pwb)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(pwb.Password.ToString());
            byte[] hash = md5.ComputeHash(inputBytes, 0, inputBytes.Length);

            PersonViewModel pvm = DataContext as PersonViewModel;
            if (pvm == null)
                throw new ArgumentException("DataContext is not set.", "DataContext");
            pvm.Password = hash;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox pwb = sender as PasswordBox;
            if(pwb == null)
                return;
            if (string.IsNullOrEmpty(pwb.Password))
                return;
            OpenReEnterPasswordDialog(pwb);
        }
    }
}
