using Bierstrichler.Data.Questions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bierstrichler.Views.Custom
{
    /// <summary>
    /// Interaction logic for SelectQuestionsToImportDialog.xaml
    /// </summary>
    public partial class SelectQuestionsToImportDialog : Window
    {
        public SelectQuestionsToImportDialog()
        {
            InitializeComponent();
            Questions = new ObservableCollection<Pair<bool, Question>>();
            Closing += SelectQuestionsToImportDialog_Closing;
            DataContext = Questions;
        }

        void SelectQuestionsToImportDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            QuestionsToImport = new List<Question>();
            foreach (Pair<bool, Question> qp in Questions)
                if (qp.First)
                    QuestionsToImport.Add(qp.Second);
        }

        public ObservableCollection<Pair<bool, Question>> Questions { get; set; }

        public List<Question> QuestionsToImport { get; set; }

        public SelectQuestionsToImportDialog(ICollection<Question> questions) : this()
        {
            foreach (Question q in questions)
            {
                if (App.Questions.Find(x => x.QuestionText.Equals(q.QuestionText)) != null)
                    Questions.Add(new Pair<bool, Question>(false, q));
                else
                    Questions.Add(new Pair<bool, Question>(true, q));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
