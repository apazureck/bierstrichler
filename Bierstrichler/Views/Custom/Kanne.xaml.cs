using System;
using System.Collections.Generic;
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
    /// Interaction logic for Kanne.xaml
    /// </summary>
    public partial class Kanne : Window
    {

        public Kanne()
        {
            InitializeComponent();
        }

        public Kanne(Functional.Answersheet AnswerSheet)
        {
            InitializeComponent();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Richtige Antworten:\n");
            foreach (var answer in AnswerSheet.SelectedQuestion.Answers)
                if (answer.Second)
                    sb.AppendLine("- " + answer.First);
            rightAnswers.Text = sb.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
