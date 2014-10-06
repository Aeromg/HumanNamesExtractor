using System;
using System.Text.RegularExpressions;

namespace IndexerLib.Pdf
{
    class SimplePdfTextCleaner : ITextCleaner
    {
        private static readonly Regex SpacesRegex = new Regex(@"\s{2,}", RegexOptions.Multiline);
        private static readonly Regex WrapRegex = new Regex(@"\-(\s*)\n", RegexOptions.Multiline);
        private static readonly Regex BreakRegex = new Regex(@"(?<![\.\:])\s*\n", RegexOptions.Multiline);

        public string Clean(string result)
        {
            var none = String.Empty;

            result = WrapRegex.Replace(result, none);
            result = SpacesRegex.Replace(result, " ");
            result = BreakRegex.Replace(result, none);

            return result;
        }
    }
}
