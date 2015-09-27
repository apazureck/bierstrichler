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

namespace Bierstrichler.Views
{
    /// <summary>
    /// Interaction logic for AlphabeticalBar.xaml
    /// </summary>
    public partial class AlphabeticalBar : UserControl
    {
        public AlphabeticalBar()
        {
            InitializeComponent();
            CreateButtons();
        }

        private void CreateButtons()
        {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            foreach(char letter in alpha)
            {
                Button b = new Button();
                b.Content = letter;
                b.Width = 15;
                sp.Children.Add(b);
            }
        }
    }
}
