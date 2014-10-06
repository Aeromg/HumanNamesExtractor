using IndexerLib.Lingva;
using NPetrovich;
using System;
using System.Text.RegularExpressions;

namespace IndexerLib.RegexExtension
{
    [RegexFunctionInfo("i")]
    class InflectRegexFunction : IRegexFunction
    {
        private readonly MyOwnPetrovich _petrovich = new MyOwnPetrovich();

        private static readonly Regex ArgumentPattern = new Regex(@"\b[а-яёА-ЯЁ]+\b");

        public string Execute(string argument)
        {
            if (argument == null)
                throw new ArgumentNullException("argument");

            if (argument.Trim().Length == 0 || !ArgumentPattern.IsMatch(argument))
                return argument;

            return InflateAndReturnRegexPattern(argument);
        }

        string InflateAndReturnRegexPattern(string word)
        {
            _petrovich.SetNominative(word);
            _petrovich.SetGender(Gender.Male);

            var words = new []
            {
                _petrovich.InflectFirstNameTo(Case.Accusative),
                _petrovich.InflectFirstNameTo(Case.Dative),
                _petrovich.InflectFirstNameTo(Case.Genitive),
                _petrovich.InflectFirstNameTo(Case.Instrumental),
                _petrovich.InflectFirstNameTo(Case.Prepositional),
                word
            };

            return
                String.Format("({0})", String.Join("|", words));
        }
    }
}
