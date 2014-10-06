using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndexerLib.Lingva
{
    class NamesTokenizer : ITokenizer
    {
        private static readonly Regex NamesRegex = new Regex(@"(?<!(ул|им)\.\s)((?<![А-ЯЁ«\-])([А-ЯЁ]([а-яё]+|\.)\s+){1,3}[А-ЯЁ]([а-яё]+)(?![»]))", RegexOptions.Multiline);
        private static readonly Regex LookupBoundaryRegex = new Regex(@"(^|((?<=[\w\d])[\.\?\!]\s*))(?=[А-ЯЁ\d])");
        private const int LookupBoundaryStepSize = 0x80;

        public IEnumerable<Token> GetTokens(string text)
        {
            var matches = NamesRegex.Matches(text).Cast<Match>();
            var tokens = matches.Select(m => new Token()
            {
                Text = m.Value,
                Lookup = GetLookup(text, m.Value, m.Index),
                Index = m.Index
            });

            return Filter(tokens);
        }

        static string GetLookup(string text, string token, int index)
        {
            var begining = GetLookupBeginingIndex(text, index);
            var ending = GetLookupEndingIndex(text, index + token.Length);

            return text.Substring(begining, ending - begining);
        }

        static int GetLookupBeginingIndex(string text, int index)
        {
            index -= LookupBoundaryStepSize;

            while (index >= 0)
            {
                var result = SearchForLookupBoundaryIndex(text, index);
                if (result > 0)
                    return result;

                index -= LookupBoundaryStepSize;
            }

            return 0;
        }

        static int GetLookupEndingIndex(string text, int index)
        {
            index += LookupBoundaryStepSize;

            while (index < text.Length)
            {
                var result = SearchForLookupBoundaryIndex(text, index);
                if (result > 0)
                    return result;

                index += LookupBoundaryStepSize;
            }

            return text.Length;
        }

        static int SearchForLookupBoundaryIndex(string text, int starting)
        {
            if (text.Length < starting + LookupBoundaryStepSize)
                return text.Length;

            var match = LookupBoundaryRegex.Match(text, starting, LookupBoundaryStepSize);
            return match.Index + match.Length;
        }

        static IEnumerable<Token> Filter(IEnumerable<Token> tokens)
        {
            var groups = tokens.GroupBy(t => t.LowerText);
            return groups.Select(g => g.First());
        }
    }
}
