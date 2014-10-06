using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using IndexerLib.Persist;

namespace IndexerLib.Lingva
{
    public class NamesExtractor
    {

        static readonly string[] MiddleNameEndings = { "ович", "евич", "овна", "евна", "ична" };

        private readonly ITokenizer _namesTokenizer;

        public NamesExtractor(ITokenizer namesTokenizer)
        {
            _namesTokenizer = namesTokenizer;
        }

        public IEnumerable<NamesExtractionResult> ExtractNames(string text)
        {
            // ToDo: разобрать этот спагетти-код!
            var results = new ConcurrentDictionary<string, NamesExtractionResult>();

            // всё, похожее на людей
            var tokens = GetTokens(text).ToArray().AsEnumerable();

            // известные люди
            var tokenToKnownPersons = GetKnownPersons(tokens);
            var knownPersonsTokens = tokenToKnownPersons.Keys;

            foreach (var tokenPersonPair in tokenToKnownPersons)
            {
                var person = new Person() 
                {
                    FirstName = tokenPersonPair.Value.NominativeName,
                    LastName = tokenPersonPair.Value.NominativeSurname
                };

                AppendSearchResult(tokenPersonPair.Key, person, NameProbability.KnownPerson, results);
            }

            var a = results.ToArray();

            tokens = tokens.Except(tokenToKnownPersons.Select(k => k.Key), new TokenEqualityComparer());

            // стоп-слова
            var names = RemoveStopWords(tokens);

            // имена в номинативной форме
            var nominativeNames = GetNominativeFullNames(names);
            nominativeNames.AsParallel().ForAll(token =>
            {
                var person = Person.Parse(token.Text);
                // если знаем фамилию или она не похожа на отчество
                if (CheckIsNominativeKnownLastName(person.LastName) || !CheckLastNameIsMiddleName(person.LastName))
                    AppendSearchResult(token, person, NameProbability.NominativeName, results);
            });

            names = names.Except(results.Values.Select(r => r.Token), new TokenEqualityComparer());

            // оставшийся мусор и имена в отличных от номинативной формах
            names.AsParallel().ForAll(token =>
            {
                var person = Person.Parse(token.Text);
                AppendSearchResult(token, person, NameProbability.MatchPattern, results);
            });

            return results.Values;
        }

        private static bool CheckIsNominativeKnownLastName(string lastName)
        {
            return KnownPersonSearcher.SearchByLastName(lastName).Any(r => r.Case == @"Nominative");
        }

        private static bool CheckLastNameIsMiddleName(string lastName)
        {
            return MiddleNameEndings.Any(lastName.EndsWith);
        }

        private IEnumerable<Token> GetTokens(string text)
        {
            return _namesTokenizer.GetTokens(text);
        }

        private static IEnumerable<Token> RemoveStopWords(IEnumerable<Token> tokens)
        {
            return tokens.Where(t => !CheckIfStopWord(t));
        }

        private static bool CheckIfStopWord(Token token)
        {
            return
                (from stopWord in Context.Cached.StopWords
                where stopWord.Regex.IsMatch(token.LowerText)
                select token).FirstOrDefault() != null;
        }

        private static IEnumerable<Token> GetNominativeFullNames(IEnumerable<Token> tokens)
        {
            return
                from token in tokens
                where CheckIfLikeNominativeFullName(token.LowerText)
                select token;
        }

        private static bool CheckIfLikeNominativeFullName(string fullname)
        {
            var names = SplitNames(fullname);
            return names.Any(n => SearchForNominativeName(n) == n);
        }

        private static IEnumerable<string> SplitNames(string fullName)
        {
            return fullName.Split(' ');
        }

        private static string SearchForNominativeName(string name)
        {
            var knownName = KnownNamesSearcher.Search(name);
            return knownName != null ? knownName.KnownName.Nominative : "";
        }

        private static IDictionary<Token, KnownPerson> GetKnownPersons(IEnumerable<Token> tokens)
        {
            var results = new Dictionary<Token, KnownPerson>();
            foreach (var token in tokens)
            {
                var knownPerson = KnownPersonSearcher.Search(Person.Parse(token.LowerText));
                if (knownPerson != null)
                    results.Add(token, knownPerson);
            }

            return results;
        }

        private static void AppendSearchResult(
            Token token, 
            Person person, 
            NameProbability probability,
            IDictionary<string, NamesExtractionResult> result)
        {
            var fullName = person.FullName;
            if (result.ContainsKey(fullName))
                return;

            result[fullName] = new NamesExtractionResult()
            {
                Person = person,
                Probability = probability,
                Token = token
            };
        }
    }
}
