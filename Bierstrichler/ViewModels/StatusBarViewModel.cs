using Bierstrichler.Commands;
using Bierstrichler.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;

namespace Bierstrichler.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        public StatusBarViewModel(StatusBar View)
        {
            view = View;
            View.DataContext = this;
            resetTimer = new Timer();
            resetTimer.Elapsed += resetTimer_Elapsed;
            resetTimer.AutoReset = false;
        }

        private StatusBar view;

        private Timer resetTimer;
        void resetTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ResetProgressBar();
        }

        #region Properties

        private string progressBarText;
        /// <summary>
        /// Updates the Status Text
        /// </summary>
        public string ProgressBarText
        {
            get { return progressBarText; }
            set
            {
                progressBarText = value;
                RaisePropertyChanged();
            }
        }

        private double progress;
        /// <summary>
        /// Value on Status bar from 1 - 100.
        /// </summary>
        public double Progress
        {
            get { return progress; }
            set
            {
                if (value > 100.0)
                    value = 100.0;
                if (value < 0.0)
                    value = 0.0;
                progress = value;
                RaisePropertyChanged();
            }
        }

        #endregion Properties

        new internal void SetProgressBar(string progressText, double progress)
        {
            ProgressBarText = progressText;
            Progress = progress;
        }

        internal void ResetProgressBar()
        {
            ProgressBarText = "";
            Progress = 0.0;
        }

        /// <summary>
        /// Resets the Progress bar after the <paramref name="wait"/> time.
        /// </summary>
        /// <param name="wait">Waiting time in ms</param>
        new internal void ResetProgressBar(double wait)
        {
            resetTimer.Interval = wait;
            resetTimer.Start();
        }

        new internal void UpdateProgress(int current, int max)
        {
            Progress = (double)current / (double)max * 100.0;
        }

        private string currentUser;

        public string CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = "Verantwortlicher: " + value;
                RaisePropertyChanged();
            }
        }

        private ICommand changeUserCommand;
        public ICommand ChangeUserCommand
        {
            get
            {
                if (changeUserCommand == null)
                    changeUserCommand = new RelayCommand(param => ChangeUser_Command(param));
                return changeUserCommand;
            }
            set
            {
                changeUserCommand = value;
            }
        }

        private void ChangeUser_Command(object param)
        {
            LoginBox lb = new LoginBox();
            if (lb.ShowDialog() == true)
            {
                App.CurrentVendor = lb.FoundUser;
            }
        }
    }
}
