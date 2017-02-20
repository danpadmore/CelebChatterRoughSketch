using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApplication.Model
{
    public class Character 
    {
        private readonly List<QuestionAndAnswer> _questionAndAnswers;

        public Guid Id { get; private set; }
        public string Name { get; private set; }

        public IReadOnlyList<QuestionAndAnswer> QuestionAndAnswers 
        {
            get { return _questionAndAnswers.AsReadOnly(); }
        }

        public Character(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Id = Guid.NewGuid();
            _questionAndAnswers = new List<QuestionAndAnswer>();
        }

        public void AddQuestionAndAnswer(string question, string answer)
        {
            var questionAndAnswer = new QuestionAndAnswer(Clean(question)){ Answer = Clean(answer) };
            AddQuestionAndAnswer(questionAndAnswer);
        }

        public void AddQuestionAndAnswer(QuestionAndAnswer questionAndAnswer)
        {
            if(_questionAndAnswers.Any(q => q.Question == questionAndAnswer.Question))
                return;

            _questionAndAnswers.Add(questionAndAnswer);
        }

        public void AddQuestion(Character questioner, string question)
        {
            if (questioner == null) throw new ArgumentNullException(nameof(questioner));
            if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException(nameof(question));

            if(_questionAndAnswers.Any(q => q.Question == question))
                return;

            _questionAndAnswers.Add(new QuestionAndAnswer(question));
        }

        public void AddAnswer(string question, string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) throw new ArgumentNullException(nameof(answer));
            if (string.IsNullOrWhiteSpace(question)) throw new ArgumentNullException(nameof(question));

            if(!_questionAndAnswers.Any(q => q.Question == question))
                throw new InvalidOperationException($"Unknown question '{question}'");
        }

                private string Clean(string value)
        {
            return value.Replace("\r", "").Replace("\n", "").Trim();
        }
    }
}