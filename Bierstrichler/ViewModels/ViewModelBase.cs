using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChangedForAll()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }

        protected void SetProgressBar(string text, double value)
        {
            App.StatusBar.SetProgressBar(text, value);
        }

        protected void UpdateProgress(int current, int max)
        {
            App.StatusBar.UpdateProgress(current, max);
        }

        protected void ResetProgressBar(double wait)
        {
            App.StatusBar.ResetProgressBar(wait);
        }
    }
}
