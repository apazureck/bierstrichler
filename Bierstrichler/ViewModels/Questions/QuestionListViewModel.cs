using Bierstrichler.Commands;
using Bierstrichler.Data.Questions;
using Bierstrichler.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bierstrichler.ViewModels.Questions
{
    public class QuestionListViewModel : ViewModelBase
    {
        public QuestionListViewModel(IList<Question> models)
        {
            Model = models;
            Questions = new ObservableCollection<QuestionViewModel>();
            foreach(Question q in Model)
            {
                QuestionViewModel qvm = new QuestionViewModel(q);
                Questions.Add(qvm);
                qvm.PropertyChanged += qvm_PropertyChanged;
            }
        }

        void qvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Category"))
                Selected = sender as QuestionViewModel;
        }

        private IList<Question> Model;

        private QuestionViewModel selected;

        public QuestionViewModel Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<QuestionViewModel> Questions { get; set; }

        private ICommand addQuestionCommand;

        public ICommand AddQuestionCommand
        {
            get
            {
                if (addQuestionCommand == null)
                    addQuestionCommand = new RelayCommand(param => AddQuestion_Command(param));
                return addQuestionCommand;
            }
            set
            {
                addQuestionCommand = value;
            }
        }

        private void AddQuestion_Command(object param)
        {
            Question q = new Question();
            q.QuestionText = "Neue Frage";
            QuestionViewModel sel = param as QuestionViewModel;
            if (sel != null)
                q.Category = sel.Category;
            Model.Add(q);
            QuestionViewModel qvm = new QuestionViewModel(q);
            qvm.PropertyChanged += qvm_PropertyChanged;
            Questions.Add(qvm);
            App.SaveQuestionsAsync();
        }

        private ICommand removeQuestionCommand;

        public ICommand RemoveQuestionCommand
        {
            get
            {
                if (removeQuestionCommand == null)
                    removeQuestionCommand = new RelayCommand(param => RemoveQuestion_Command(param));
                return removeQuestionCommand;
            }
            set
            {
                removeQuestionCommand = value;
            }
        }

        private void RemoveQuestion_Command(object param)
        {
            QuestionViewModel qvm = param as QuestionViewModel;
            if (qvm == null)
                return;
            Questions.Remove(qvm);
            Model.Remove(qvm.Model);
            App.SaveQuestionsAsync();
        }

        private ICommand changeCategoryCommand;

        public ICommand ChangeCategoryCommand
        {
            get
            {
                if (changeCategoryCommand == null)
                    changeCategoryCommand = new RelayCommand(param => ChangeCategory_Command(param));
                return changeCategoryCommand;
            }
            set
            {
                changeCategoryCommand = value;
            }
        }

        private void ChangeCategory_Command(object param)
        {
            TextChangedRoutedEventargs e = param as TextChangedRoutedEventargs;
            if (e == null)
                return;

            foreach (QuestionViewModel qvm in Questions)
                if (qvm.Category.Equals(e.OldText))
                    qvm.Category = e.NewText;
        }

    }
}
