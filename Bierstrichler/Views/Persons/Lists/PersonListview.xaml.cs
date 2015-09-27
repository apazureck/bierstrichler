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
    /// Interaction logic for PersonDashList.xaml
    /// </summary>
    public partial class PersonListView : UserControl
    {
        public PersonListView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is PersonViewModel)
                e.Accepted = ((PersonViewModel)e.Item).IsGuest;
            else
                e.Accepted = false;
        }

        public bool RequestPasswordOnDrop
        {
            get { return (bool)GetValue(RequestPasswordOnDropProperty); }
            set { SetValue(RequestPasswordOnDropProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestPasswordOnDrop.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestPasswordOnDropProperty =
            DependencyProperty.Register("RequestPasswordOnDrop", typeof(bool), typeof(PersonListView), new PropertyMetadata(false));


    }
}
