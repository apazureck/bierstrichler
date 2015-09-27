using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bierstrichler.Data.Questions
{
    [Serializable]
    public class Question
    {
        public Question()
        {
            Answers = new List<Pair<string, bool>>();
            QuestionText = "";
            RightAnswerResponse = "";
            WrongAnswerResponse = "";
            Category = "";
        }
        public List<Pair<string, bool>> Answers { get; set; }

        public string QuestionText { get; set; }

        public string RightAnswerResponse { get; set; }

        public string WrongAnswerResponse { get; set; }

        public string Category { get; set; }

        public int RightAnswerCount { get; set; }

        public int TotalAnswerCount { get; set; }

        public bool CheckAnswer(IList<Pair<string, bool>> answers)
        {
            bool isAnswerRight = true;
            if (answers.Count < 1)
                isAnswerRight = false;
            foreach(Pair<string, bool> answer in answers)
            {
                Pair<string, bool> rightAnswer = Answers.Find(x => x.First.Equals(answer.First));
                if (rightAnswer != null)
                    isAnswerRight = (rightAnswer.Second == answer.Second) && isAnswerRight;
                else
                    isAnswerRight = false;
            }
            RightAnswerCount += isAnswerRight ? 1 : 0;
            TotalAnswerCount++;
            return isAnswerRight;
        }
    }
}
