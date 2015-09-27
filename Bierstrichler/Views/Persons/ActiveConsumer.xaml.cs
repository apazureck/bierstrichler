using Bierstrichler.ViewModels;
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
    /// Interaction logic for ActiveConsumer.xaml
    /// </summary>
    public partial class ActiveConsumer : UserControl
    {
        public ActiveConsumer()
        {
            InitializeComponent();
        }

        private void RecentlyUsedList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PersonViewModel oldVM = e.OldValue as PersonViewModel;
            PersonViewModel newVM = e.NewValue as PersonViewModel;
            if (oldVM != null)
                oldVM.StopRefreshRecentDrinkListUpdateTimer();
            if (newVM != null)
                newVM.StartRefreshRecendDrinkListUpdateTimer();
        }
    }
}
