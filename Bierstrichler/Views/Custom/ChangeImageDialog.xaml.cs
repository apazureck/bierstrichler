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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bierstrichler.Views.Custom
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class ChangeImageDialog : Window
    {
        public ChangeImageDialog(BitmapImage source, Bierstrichler.Data.Global.Rectangle rect)
        {
            InitializeComponent();
            webCamImage.Source = source;
            webCamImage.Width = source.Width;
            webCamImage.Height = source.Height;

            sizeRect.Height = rect.Height;
            sizeRect.Width = rect.Width;
            Canvas.SetLeft(sizeRect, rect.X);
            Canvas.SetTop(sizeRect, rect.Y);
            this.Closing += CameraWindowDialog_Closing;
            foreach (Control child in canv.Children)
                Selector.SetIsSelected(child, true);
        }

        void CameraWindowDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FitInContainer();
            ImageCutOut = new Bierstrichler.Data.Global.Rectangle(Canvas.GetLeft(sizeRect), Canvas.GetTop(sizeRect), sizeRect.Width, sizeRect.Height);
            DialogResult = true;
        }

        void FitInContainer()
        {
            if (Canvas.GetTop(sizeRect) < 0)
                Canvas.SetTop(sizeRect, 0);
            if (Canvas.GetLeft(sizeRect) < 0)
                Canvas.SetLeft(sizeRect, 0);
            if (Canvas.GetLeft(sizeRect) + sizeRect.Width > webCamImage.Width)
                sizeRect.Width = webCamImage.Width - Canvas.GetLeft(sizeRect);
            if (Canvas.GetTop(sizeRect) + sizeRect.Height > webCamImage.Height)
                sizeRect.Height = webCamImage.Height - Canvas.GetTop(sizeRect);

        }

        public Bierstrichler.Data.Global.Rectangle ImageCutOut { get; private set; }
    }
}
