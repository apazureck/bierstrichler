using Bierstrichler.Data.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bierstrichler;
using System.Collections.ObjectModel;
using Bierstrichler.Views.Questions;
using System.Windows;
using Bierstrichler.Functional.ExtensionMethods;
using Bierstrichler.Views.Custom;

namespace Bierstrichler.Functional
{
    static class QuestionMaster
    {
        private static IList<Question> Questions { get {return App.Questions; } }

        static int MinClicks { get { return Bierstrichler.Properties.Settings.Default.QuestionMinClicks; } }

        static int MaxClicks { get { return Bierstrichler.Properties.Settings.Default.QuestionMaxClicks; } }

        public static int NextQuestionClicks { get; private set; }

        public static void Hit()
        {
            NextQuestionClicks--;
            if (NextQuestionClicks < 1)
            {
                OpenQuestionDialog();
                Random r = new Random();
                NextQuestionClicks = r.Next(MinClicks, MaxClicks);
            }                
        }

        static void OpenQuestionDialog()
        {
            QuestionDialog qd = new QuestionDialog();
            Answersheet AnswerSheet = new Answersheet(SelectQuestion());
            qd.DataContext = AnswerSheet;
            qd.ShowDialog();
            if (!AnswerSheet.CheckAnswer())
            {
                Kanne kd = new Kanne(AnswerSheet);
                kd.ShowDialog();
            }
            App.SaveQuestionsAsync();
        }

        private static Question SelectQuestion()
        {
            Random r = new Random();
            int questionNumber = r.Next(0, Questions.Count - 1);
            return Questions[questionNumber];
        }

        static public void Reset()
        {
            Random r = new Random();
            NextQuestionClicks = r.Next(MinClicks, MaxClicks);
        }
    }

    public class Answersheet
    {
        public Answersheet(Question q)
        {
            SelectedQuestion = q;
            Answerkeys = new ObservableCollection<Pair<string, bool>>();
            GivenAnswers = new ObservableCollection<string>();
            List<Pair<string, bool>> tAnswer = new List<Pair<string, bool>>(q.Answers);
            tAnswer.Shuffle();
            foreach (Pair<string, bool> answer in tAnswer)
                Answerkeys.Add(new Pair<string, bool>(answer.First, false));
        }
        public Question SelectedQuestion { get; set; }

        public ObservableCollection<Pair<string, bool>> Answerkeys { get; set; }

        public ObservableCollection<string> GivenAnswers { get; set; }

        internal bool CheckAnswer()
        {
            return SelectedQuestion.CheckAnswer(Answerkeys);
        }
    }
}
