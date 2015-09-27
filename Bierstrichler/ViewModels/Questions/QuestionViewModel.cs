using Bierstrichler.Commands;
using Bierstrichler.Data.Questions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bierstrichler.ViewModels.Questions
{
    public class QuestionViewModel : ViewModelBase
    {
        public QuestionViewModel(Question model)
        {
            Model = model;
            Answers = new ObservableCollection<PairViewModel>();
            foreach (Pair<string, bool> answer in Model.Answers)
                Answers.Add(new PairViewModel(answer));
            PropertyChanged += QuestionViewModel_PropertyChanged;
        }

        void QuestionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            App.SaveQuestionsAsync();
        }

        #region Properties
        public Question Model { get; set; }

        public string QuestionText
        {
            get { return Model.QuestionText; }
            set
            {
                Model.QuestionText = value;
                RaisePropertyChanged();
            }
        }

        public string RightAnswerResponse
        {
            get { return Model.RightAnswerResponse; }
            set
            {
                Model.RightAnswerResponse = value;
                RaisePropertyChanged();
            }
        }

        public string WrongAnswerResponse
        {
            get { return Model.WrongAnswerResponse; }
            set
            {
                Model.WrongAnswerResponse = value;
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get { return Model.Category; }
            set
            {
                Model.Category = value;
                RaisePropertyChanged();
            }
        }

        public int TotalAnswerCount
        {
            get { return Model.TotalAnswerCount; }
        }

        public int RightAnswerCount
        {
            get { return Model.RightAnswerCount; }
        }

        public ObservableCollection<PairViewModel> Answers {get; set;}

        #endregion Properties

        #region Commands

        private ICommand addAnswerCommand;

        public ICommand AddAnswerCommand
        {
            get
            {
                if (addAnswerCommand == null)
                    addAnswerCommand = new RelayCommand(param => AddAnswer_Command(param));
                return addAnswerCommand;
            }
            set
            {
                addAnswerCommand = value;
            }
        }

        private void AddAnswer_Command(object param)
        {
            Pair<string, bool> answer = new Pair<string, bool>("Neue Antwort", false);
            Model.Answers.Add(answer);
            Answers.Add(new PairViewModel(answer));
            App.SaveQuestionsAsync();
        }

        private ICommand removeAnswerCommand;

        public ICommand RemoveAnswerCommand
        {
            get
            {
                if (removeAnswerCommand == null)
                    removeAnswerCommand = new RelayCommand(param => RemoveAnswer_Command(param));
                return removeAnswerCommand;
            }
            set
            {
                removeAnswerCommand = value;
            }
        }

        private void RemoveAnswer_Command(object param)
        {
            PairViewModel answer = param as PairViewModel;
            if (answer == null)
                return;
            Answers.Remove(answer);
            Model.Answers.Remove(answer.Model);
            App.SaveQuestionsAsync();
        }

        #endregion Commands
    }
}
