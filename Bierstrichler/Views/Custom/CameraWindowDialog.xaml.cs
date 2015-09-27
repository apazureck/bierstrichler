using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections.Generic;
using System.IO;
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
using WebcamControl;

namespace Bierstrichler.Views.Custom
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindowDialog : Window
    {
        public CameraWindowDialog()
        {
            InitializeComponent();
            webCamImage.VideoDevice = EncoderDevices.FindDevices(EncoderDeviceType.Video).FirstOrDefault(x => x.Name == Bierstrichler.Properties.Settings.Default.WebCamSource);
            Width = 640;
            Height = 480;
            webCamImage.StartPreview();
            this.Closing += CameraWindowDialog_Closing;
        }

        void CameraWindowDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(tempPath))
                this.DialogResult = false;
            else
                this.DialogResult = true;
            webCamImage.StopPreview();
            //FitInContainer();
            //ImageCutOut = new Bierstrichler.Data.Global.Rectangle(Canvas.GetLeft(sizeRect), Canvas.GetTop(sizeRect), sizeRect.Width, sizeRect.Height);
        }

        

        public Bierstrichler.Data.Global.Rectangle ImageCutOut { get; private set; }

        public string tempPath { get; private set; }

        private void TakePictureButton_Click(object sender, RoutedEventArgs e)
        {
            webCamImage.ImageDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Bierstrichler");
            if (!Directory.Exists(webCamImage.ImageDirectory))
                Directory.CreateDirectory(webCamImage.ImageDirectory);
            tempPath = webCamImage.TakeSnapshot();
            Close();
        }
    }
}
