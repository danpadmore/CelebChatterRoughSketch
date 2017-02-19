using System;

namespace ConsoleApplication.Model
{
    public class QuestionAndAnswer
    {
        public string Answer { get; set; }
        public string Question { get; private set; }
        

        public QuestionAndAnswer(string question)
        {
            if(question == null) throw new ArgumentNullException(nameof(question));

            Question = question;
        }
    }
}