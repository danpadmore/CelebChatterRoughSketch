using System.Diagnostics;
using static System.Console;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleApplication.ImsdbAnalysis;

namespace ConsoleApplication
{
    public class Program
    {
        private const string DefaultScriptUrl = "http://www.imsdb.com/scripts/Interstellar.html";
        private const string DefaultOutputPath = "/home/dan/Downloads/chatter/qna.txt";

        public static void Main(string[] args)
        {
            try 
            {
                WriteLine();
                WriteLine("Enter movie script url (use format http://www.imsdb.com/scripts/<selected movie>.html):");

                string scriptUrl = null;
                if(!Debugger.IsAttached)
                    scriptUrl = ReadLine();

                if(string.IsNullOrWhiteSpace(scriptUrl))
                {
                    scriptUrl = DefaultScriptUrl;
                    WriteLine($"No movie script url entered, using default '{DefaultScriptUrl}'");
                }

                WriteLine("Analyzing movie script...");
                var analysisResult = AnalyzeMovieScript(scriptUrl).Result;

                var chosenCharacter = string.Empty;
                while(!analysisResult.CharactersWithAnswer.Any(c => string.Equals(c.Name, chosenCharacter, StringComparison.CurrentCultureIgnoreCase)))
                {
                    WriteLine();
                    WriteLine("We found these characters, enter the name of the character you would like to chat to:");
                    analysisResult.CharactersWithAnswer.ForEach(c => WriteLine($" - {c.Name} ({c.QuestionAndAnswers.Count} questions and answers)"));

                    if(!Debugger.IsAttached)
                        chosenCharacter = ReadLine();
                    else
                        chosenCharacter = analysisResult.CharactersWithAnswer.OrderByDescending(c => c.QuestionAndAnswers.Count).First().Name;
                }

                WriteLine(@"Enter file path to output questions and answers for QnA Maker (e.g. C:\temp\qna.txt)");

                string outputPath = null;
                if(!Debugger.IsAttached)
                    outputPath = ReadLine();
                else   
                    outputPath = DefaultOutputPath;

                if(string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = DefaultOutputPath;
                    WriteLine($"No file path entered, using default '{DefaultOutputPath}'");
                }

                var character = analysisResult.CharactersWithAnswer.Single(c => string.Equals(c.Name, chosenCharacter, StringComparison.CurrentCultureIgnoreCase));
                File.Delete(outputPath);
                File.AppendAllLines(outputPath, character.QuestionAndAnswers.Select(q => $@"{q.Question}{Environment.NewLine}{q.Answer}{Environment.NewLine}"));

                WriteLine("Go to https://qnamaker.ai/Create and upload the result under FAQ Files");
            }
            catch(Exception ex)
            {
                WriteLine(ex);
            }
            finally
            {
                WriteLine();
                WriteLine("End");

                if(!Debugger.IsAttached)
                    ReadLine();
            }
        }

        private static async Task<AnalysisResult> AnalyzeMovieScript(string scriptUrl)
        {
            var analyzer = new Analyzer();

            using(var httpClient = new HttpClient())
            {
                var movieScript = await httpClient.GetStringAsync(scriptUrl);

                return analyzer.Analyze(movieScript);
        }
    }
}
}