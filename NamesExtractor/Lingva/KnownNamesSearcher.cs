using System;
using IndexerLib.Persist;
using NPetrovich;

namespace IndexerLib.Lingva
{
    public static class KnownNamesSearcher
    {
        public class KnownNamesSearchResult
        {
            public KnownName KnownName { get; set; }

            public string Case { get; set; }
        }

        static Case GetCaseForFirstName(KnownName knownName, string casedName)
        {
            if (casedName.Equals(knownName.Nominative))
                return Case.Nominative;

            if (casedName.Equals(knownName.Accusative))
                return Case.Accusative;

            if (casedName.Equals(knownName.Dative))
                return Case.Dative;

            if (casedName.Equals(knownName.Genitive))
                return Case.Genitive;

            if (casedName.Equals(knownName.Instrumental))
                return Case.Instrumental;

            if (casedName.Equals(knownName.Prepositional))
                return Case.Prepositional;

            return Case.Nominative;
        }

        static KnownName TryGetKnownName(string name)
        {
            KnownName knownName;
            Context.Cached.KnownNamesHash.TryGetValue(name, out knownName);

            return knownName;
/*
            return 
                (from knownName in Context.Cached.KnownNames
                 where
                    knownName.Accusative == name ||
                    knownName.Dative == name ||
                    knownName.Genitive == name ||
                    knownName.Instrumental == name ||
                    knownName.Nominative == name ||
                    knownName.Prepositional == name
                 select knownName).FirstOrDefault();*/
        }

        public static KnownNamesSearchResult Search(string name)
        {
            name = name.Trim().ToLower().Replace('ё', 'е').Replace('Ё', 'Е');

            var knownName = TryGetKnownName(name);
            if (knownName == null)
                return null;

            var @case = GetCaseForFirstName(knownName, name);

            return new KnownNamesSearchResult()
            {
                KnownName = knownName,
                Case = Enum.GetName(typeof (Case), @case)
            };
        }

        public static bool CheckIfMale(string firstName)
        {
            firstName = firstName.Replace('ё', 'е').Replace('Ё', 'Е');
            var knownName = Search(firstName);
            return knownName == null || knownName.KnownName.IsMale;
        }
    }
}
