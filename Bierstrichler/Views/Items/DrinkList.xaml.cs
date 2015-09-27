using Bierstrichler.Data.Items;
using Bierstrichler.Functional;
using Bierstrichler.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Bierstrichler.Views.Items
{
    /// <summary>
    /// Interaction logic for Getraenkeliste.xaml
    /// </summary>
    public partial class DrinkList : UserControl
    {
        public DrinkList()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is ItemViewModel)
                e.Accepted = ((ItemViewModel)e.Item).Model.Available;
            else
                e.Accepted = false;
        }
    }
}
