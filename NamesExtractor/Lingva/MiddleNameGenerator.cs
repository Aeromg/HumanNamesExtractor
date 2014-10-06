using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerLib.Lingva
{
    class MiddleNameGenerator
    {
        private static readonly string[] ConsonantsLetters = new[]
        {
            "б", "в", "г", "д", "ж", "з", "к", "л", "м", "н", "п", "р", "с", "т", "ф", "х", "ц", "ч", "ш", "щ"
        };

        private static readonly string[] VowelsLetters = new[]
        {
            "а", "е", "ё", "и", "о", "у", "ы", "э", "ю", "я"
        };

        private static readonly string[] OpenedVowelsLetters = new[]
        {
            "а", "у", "ы"
        };

        private static readonly string[] WindConsonantsLetters = new[]
        {
            "ж", "ш", "ч", "щ", "ц"
        };

        private const string AlteratorSign = "ь";

        private const string SplitterSign = "й";

        private static readonly string[] AlteredConsonantsLetters;

        private static readonly string[] SplittedVowelsLetters;

        private static readonly string[] StrongConsonantsLetters;

        static MiddleNameGenerator()
        {
            AlteredConsonantsLetters = ConsonantsLetters.Select(l => l + AlteratorSign).ToArray();
            SplittedVowelsLetters = VowelsLetters.Select(l => l + SplitterSign).ToArray();
            StrongConsonantsLetters = ConsonantsLetters.Where(l => !WindConsonantsLetters.Contains(l)).ToArray();
        }
    }
}
