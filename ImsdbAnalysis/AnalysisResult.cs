using System;
using System.Linq;
using System.Collections.Generic;
using ConsoleApplication.Model;

namespace ConsoleApplication.ImsdbAnalysis
{
    public class AnalysisResult
    {
        public List<Character> Characters {get; private set;}
        public List<Character> CharactersWithAnswer 
        {
            get
            {
                return Characters.Where(c => c.QuestionAndAnswers.Any()).ToList();
            }
        }

        public AnalysisResult()
        {
            Characters = new List<Character>();
        }

        public void AddCharacterIfNotExist(string characterName)
        {
            if(Characters.Any(c => string.Equals(c.Name, characterName, StringComparison.OrdinalIgnoreCase)))
                return;

            Characters.Add(new Character(characterName));
        }

        public void AddQuestionAndAnswer(string characterName, string question, string answer)
        {
            var character = Characters.SingleOrDefault(c => string.Equals(c.Name, characterName, StringComparison.OrdinalIgnoreCase));
            if(character == null)
                return;

            character.AddQuestionAndAnswer(question, answer);
        }
    }
}