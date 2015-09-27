using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Bierstrichler.Events
{
    public class TextChangedRoutedEventargs : RoutedEventArgs
    {
        public string OldText { get; private set; }
        public string NewText { get; private set; }

        public TextChangedRoutedEventargs(RoutedEvent re, string oldValue, string newValue) : base(re)
        {
            OldText = oldValue;
            NewText = newValue;
        }
    }
}
