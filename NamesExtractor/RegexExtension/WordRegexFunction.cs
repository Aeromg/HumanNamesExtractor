namespace IndexerLib.RegexExtension
{
    [RegexFunctionInfo("w")]
    public class WordRegexFunction : IRegexFunction
    {
        public string Execute(string argument)
        {
            return @"\b" + argument + @"\b";
        }
    }
}
