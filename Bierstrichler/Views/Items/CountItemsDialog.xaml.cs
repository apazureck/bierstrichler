using Bierstrichler.Data.Items;
using Bierstrichler.ViewModels.Items;
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
using System.Windows.Shapes;

namespace Bierstrichler.Views.Items
{
    /// <summary>
    /// Interaction logic for CountItemsDialog.xaml
    /// </summary>
    public partial class CountItemsDialog : Window
    {
        public CountItemsDialog()
        {
            InitializeComponent();
        }
        public CountItemsDialog(ItemListViewModel items)
        {
            InitializeComponent();
            ItemCountingListViewModel iclvm = new ItemCountingListViewModel(items.GetModel());
            this.DataContext = iclvm;
            iclvm.CountingDone += iclvm_CountingDone;
            iclvm.CountingCancelled += iclvm_CountingCancelled;
        }

        void iclvm_CountingCancelled(object sender, EventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        void iclvm_CountingDone(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!DialogResult == true)
                if (MessageBox.Show("Wollen Sie den Zählvorgang wirklich abbrechen?", "Zählvorgang wird abgebrochen", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    e.Cancel = true;
        }
    }
}
