namespace ConsoleApplication.ImsdbAnalysis
{
    public class AnalysisContext 
    {
        public string CharacterName { get; private set; }

        public string Question { get; private set; }

        public bool IsQuestionPending { get; private set; }

        public void SetCharacterName(string characterName)
        {
            CharacterName = characterName;
        }

        public void SetQuestion(string question)
        {
            Question = question.Trim();
            IsQuestionPending = true;
        }

        public void Reset()
        {
            CharacterName = null;
            Question = null;
            IsQuestionPending = false;
        }
    }
}