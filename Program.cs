using static System.Console;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using ConsoleApplication.ImsdbAnalysis;
using ConsoleApplication.Model;

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

                var scriptUrl = ReadLine();
                if(string.IsNullOrWhiteSpace(scriptUrl))
                {
                    scriptUrl = DefaultScriptUrl;
                    WriteLine($"No movie script url entered, using default '{DefaultScriptUrl}'");
                }

                WriteLine("Analyzing movie script...");
                var characters = AnalyzeMovieScript(scriptUrl).Result;

                var charactersWithAnswer = characters.Where(c => c.QuestionAndAnswers.Any()).ToList();
                var chosenCaracter = string.Empty;
                while(!charactersWithAnswer.Any(c => string.Equals(c.Name, chosenCaracter, StringComparison.CurrentCultureIgnoreCase)))
                {
                    WriteLine();
                    WriteLine("We found these characters, enter the name of the character you would like to chat to:");
                    charactersWithAnswer.ForEach(c => WriteLine($" - {c.Name} ({c.QuestionAndAnswers.Count} questions and answers)"));
                    chosenCaracter = ReadLine();
                }

                WriteLine(@"Enter file path to output questions and answers for QnA Maker (e.g. C:\temp\qna.txt)");
                var outputPath = ReadLine();
                if(string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = DefaultOutputPath;
                    WriteLine($"No file path entered, using default '{DefaultOutputPath}'");
                }

                var character = characters.Single(c => string.Equals(c.Name, chosenCaracter, StringComparison.CurrentCultureIgnoreCase));
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
                ReadLine();
            }
        }

        private static async Task<List<Character>> AnalyzeMovieScript(string scriptUrl)
        {
            var analyzer = new Analyzer();
            Character previousCharacter = null;
            var characters = new List<Character>();
            QuestionAndAnswer openQuestionAndAnswer = null;

            using(var httpClient = new HttpClient())
            {
                var movieScript = await httpClient.GetStringAsync(scriptUrl);
                var movieScriptLines = movieScript.Split(new string[]{ Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach(var movieScriptLine in movieScriptLines)
                {
                    if(movieScriptLine.Contains("What did they say?"))
                        System.Diagnostics.Debugger.Break();

                    var analysisResult = analyzer.Analyze(movieScriptLine);

                    switch (analysisResult.Type)
                    {
                        case AnalysisResultType.Character:
                            // close answer
                            if(!string.IsNullOrWhiteSpace(openQuestionAndAnswer?.Answer))
                            {
                                if(analysisResult.Value != previousCharacter?.Name)
                                    openQuestionAndAnswer = null;
                            }

                            previousCharacter = characters.SingleOrDefault(c => c.Name == analysisResult.Value);
                            if (previousCharacter != null)
                                continue;

                            previousCharacter = new Character(analysisResult.Value);
                            characters.Add(previousCharacter);
                            break;

                        case AnalysisResultType.Question:
                            openQuestionAndAnswer = new QuestionAndAnswer(analysisResult.Value);
                            break;

                        case AnalysisResultType.Dialogue:
                            if(previousCharacter == null || openQuestionAndAnswer == null)
                                continue;

                            if(string.IsNullOrWhiteSpace(openQuestionAndAnswer.Answer))
                            {
                                openQuestionAndAnswer.Answer = analysisResult.Value;
                                previousCharacter.AddQuestionAndAnswer(openQuestionAndAnswer);
                            }
                            else
                            {
                                openQuestionAndAnswer.Answer += " " + analysisResult.Value;
                            }

                            if(openQuestionAndAnswer.Answer.EndsWith("."))
                                openQuestionAndAnswer = null;
                            
                            break;

                        case AnalysisResultType.None:
                        default:
                            continue;                        
                    }
                }
            }

            return characters;
        }
    }
}
