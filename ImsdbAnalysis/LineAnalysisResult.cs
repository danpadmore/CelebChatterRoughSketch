namespace ConsoleApplication.ImsdbAnalysis
{
    public struct LineAnalysisResult
    {
        public AnalysisResultType Type { get; private set; }
        public string Value { get; private set; }

        public LineAnalysisResult(AnalysisResultType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}