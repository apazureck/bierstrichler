using Bierstrichler.Events;
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

namespace Bierstrichler.Views.Custom
{
    /// <summary>
    /// Interaction logic for EditabeTextblock.xaml
    /// </summary>
    public partial class EditabeTextblock : UserControl
    {
        // Register the routed event
        public static readonly RoutedEvent TextChangedEvent = 
            EventManager.RegisterRoutedEvent( "TextChanged", RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), typeof(EditabeTextblock));
 
        // .NET wrapper
        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); } 
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public EditabeTextblock()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }

        private string oldText = "";

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set 
            {
                if (value && !(bool)GetValue(IsEditingProperty))
                    oldText = Text;
                else if (!value && (bool)GetValue(IsEditingProperty))
                    RaiseEvent(new TextChangedRoutedEventargs(EditabeTextblock.TextChangedEvent, oldText, tbEdit.Text));
                    //TextHasChanged(this, new Events.TextChangedEventargs());

                SetValue(IsEditingProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsEditing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditabeTextblock), new PropertyMetadata(false));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditabeTextblock), new PropertyMetadata("Text"));

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount>1)
            {
                IsEditing = true;
                e.Handled = true;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                IsEditing = false;
        }

        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                IsEditing = true;
        }

        
    }
}
