using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bierstrichler.Views
{
    /// <summary>
    /// Interaction logic for DashList.xaml
    /// </summary>
    public partial class DashList : UserControl
    {
        public DashList()
        {
            InitializeComponent();
        }

        private void ShowAddUser_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation(0.0, OldWidth, TimeSpan.FromMilliseconds(200));
            allPersonsPanel.BeginAnimation(FrameworkElement.WidthProperty, da);
        }

        private double oldWidth;

        public double OldWidth
        {
            get { return oldWidth; }
            set {
                if (notinitialized)
                {
                    oldWidth = value;
                }
            }
        }

        bool notinitialized = true;

        private void ShowAddUser_Unchecked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation(OldWidth, 0.0, TimeSpan.FromMilliseconds(200));
            allPersonsPanel.BeginAnimation(FrameworkElement.WidthProperty, da);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            allPersonsPanel.Width = allPersonsPanel.ActualWidth;
            OldWidth = allPersonsPanel.Width;
            notinitialized = false;
            allPersonsPanel.Width = 0;
        }
    }
}
