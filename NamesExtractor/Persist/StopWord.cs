using IndexerLib.RegexExtension;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace IndexerLib.Persist
{
    public class StopWord : EntityBase<StopWord>
    {
        public string Pattern { get; set; }

        private Regex _regex;

        [NotMapped]
        public Regex Regex
        {
            get { 
                return _regex ?? (_regex = new Regex(RegexFunctionExecutor.ExecuteExpression(Pattern))); }
        }

        public override void BeforeSave()
        {
            Pattern = Pattern.ToLower();
        }
    }
}
