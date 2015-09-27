using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Functional
{
    public static class DialogFactory
    {
        public static OpenFileDialog GetNewImageFileDialog()
        {
            // Configure open file dialog box 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            string allCodecs = string.Empty;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, codecName, c.FilenameExtension);
                allCodecs += c.FilenameExtension + ";";
                sep = "|";
            }

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Image Files", allCodecs.Remove(allCodecs.Length-1));

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Files", "*.*");

            dlg.FilterIndex = 6;

            return dlg;
        }
    }
}
