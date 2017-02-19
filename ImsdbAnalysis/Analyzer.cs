using System.Text.RegularExpressions;

namespace ConsoleApplication.ImsdbAnalysis
{
    public class Analyzer
    {
        private static readonly LineAnalysisResult NullAnalysisResult;
        private static readonly Regex DialogueRegex;
        private static readonly Regex QuestionRegex;
        private static readonly Regex CharacterRegex;        
        
        static Analyzer()
        {
            QuestionRegex = new Regex(@"([a-zA-Z]{1,}[a-zA-Z\s,\.']{1,}\?)");
            DialogueRegex = new Regex(@"([a-zA-Z]{1,}[a-zA-Z\s,\.']{1,})");
            CharacterRegex = new Regex(@"^<b>\s*([a-zA-Z]{1,})");
            NullAnalysisResult = new LineAnalysisResult(AnalysisResultType.None, null);
        }

        public LineAnalysisResult Analyze(string movieScriptLine)
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