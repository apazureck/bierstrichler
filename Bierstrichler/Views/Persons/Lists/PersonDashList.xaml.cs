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
    public partial class PersonDashList : UserControl
    {
        public PersonDashList()
        {
            InitializeComponent();
        }

        public bool AddButtonPressed
        {
            get { return (bool)GetValue(AddButtonPressedProperty); }
            set { SetValue(AddButtonPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddButtonPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddButtonPressedProperty =
            DependencyProperty.Register("AddButtonPressed", typeof(bool), typeof(PersonDashList), new PropertyMetadata(false));

        void listView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListBox) && !(dep is ListBoxItem))
            {
                try
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                catch (Exception)
                {
                    dep = null;
                }
            }

            if (dep == null)
                return;

            if(dep is ListBox)
            {
                ((ListBox)dep).SelectedItem = null;
                e.Handled = true;
                return;
            }
        }



        public bool RequestPasswordOnAdd
        {
            get { return (bool)GetValue(RequestPasswordOnAddProperty); }
            set { SetValue(RequestPasswordOnAddProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestPasswordOnAdd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestPasswordOnAddProperty =
            DependencyProperty.Register("RequestPasswordOnAdd", typeof(bool), typeof(PersonDashList), new PropertyMetadata(true));


    }
}
