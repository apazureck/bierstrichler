using Bierstrichler.Data.Persons;
using Bierstrichler.ViewModels.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for AddNewPersonDialog.xaml
    /// </summary>
    public partial class AddNewPersonDialog : Window
    {
        public AddNewPersonDialog()
        {
            TempPerson = new PersonViewModel(new Gast());
            DataContext = this;
            InitializeComponent();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public PersonViewModel TempPerson { get; set; }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {

            if (TempPerson.Password == null)
            {
                MessageBox.Show("Kein Passwort gesetzt.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
            this.Close();
        }
    }
}
