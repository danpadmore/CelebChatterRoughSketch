using System;
using System.Text.RegularExpressions;

namespace ConsoleApplication.ImsdbAnalysis
{
    public class Analyzer
    {
        private static readonly Regex EntitiesRegex;
        private static readonly LineAnalysisResult NullAnalysisResult;
        private static readonly Regex DialogueRegex;
        private static readonly Regex QuestionRegex;
        private static readonly Regex CharacterRegex;        
        
        static Analyzer()
        {
            EntitiesRegex = new Regex(@"<b>(((?!<b>).)*)", RegexOptions.Singleline);
            QuestionRegex = new Regex(@"[^.>]*\?", RegexOptions.Singleline);
            DialogueRegex = new Regex(@"<\/b>(.*)", RegexOptions.Singleline);
            CharacterRegex = new Regex(@"<b>\s*([a-zA-Z]{1,})", RegexOptions.Singleline);
            NullAnalysisResult = new LineAnalysisResult(AnalysisResultType.None, null);
        }

        public AnalysisResult Analyze(string movieScript)
        {
            var result = new AnalysisResult();
            var context = new AnalysisContext();

            var entities = EntitiesRegex.Matches(movieScript);
            foreach(Match entity in entities)
            {
                var characterMatch = CharacterRegex.Match(entity.Value);
                if (characterMatch.Success)
                {
                    var characterName = characterMatch.Groups[1].Value.Trim();
                    context.SetCharacterName(characterName);
                    result.AddCharacterIfNotExist(characterName);
                }

                var questionMatch = QuestionRegex.Match(entity.Value);
                if (questionMatch.Success)
                {
                    context.SetQuestion(questionMatch.Value);
                }
                else if(context.IsQuestionPending)
                {
                    var dialogueMatch = DialogueRegex.Match(entity.Value);
                    if (dialogueMatch.Success)
                    {
                        var answer = dialogueMatch.Groups[1].Value;
                        result.AddQuestionAndAnswer(context.CharacterName, context.Question, answer);
                        context.Reset();
                    }   
                }
            }

            return result;
        }

        public LineAnalysisResult AnalyzeLine(string movieScriptLine)
        {
            if(string.IsNullOrWhiteSpace(movieScriptLine)) 
                return NullAnalysisResult;

            var cleanMovieScriptLine = movieScriptLine.Trim();

            var characterMatch = CharacterRegex.Match(cleanMovieScriptLine);
            if (characterMatch.Success)
                return new LineAnalysisResult(AnalysisResultType.Character, characterMatch.Groups[1].Value);
            
            var questionMatch = QuestionRegex.Match(cleanMovieScriptLine);
            if (questionMatch.Success)
                return new LineAnalysisResult(AnalysisResultType.Question, questionMatch.Groups[1].Value);

            var dialogueMatch = DialogueRegex.Match(cleanMovieScriptLine);
            if (dialogueMatch.Success)
                return new LineAnalysisResult(AnalysisResultType.Dialogue, dialogueMatch.Groups[1].Value);
           
           return NullAnalysisResult;
        }
    }
}