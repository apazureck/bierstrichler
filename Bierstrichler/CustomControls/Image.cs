using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Bierstrichler.CustomControls
{
    public class SourceIndicatorImage : Image
    {
        public SourceIndicatorImage()
        {
            this.SourceUpdated += SourceIndicatorImage_SourceUpdated;
        }

        void SourceIndicatorImage_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            IsImageSourceValid = Source == null ? false : true;
        }
        public bool IsImageSourceValid
        {
            get { return (bool)GetValue(IsImageSourceValidProperty); }
            set { SetValue(IsImageSourceValidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsImageSourceValid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImageSourceValidProperty =
            DependencyProperty.Register("IsImageSourceValid", typeof(bool), typeof(SourceIndicatorImage), new PropertyMetadata(false));

        
    }
}
