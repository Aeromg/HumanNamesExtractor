using System;
using System.Collections.Generic;
using IndexerLib.Persist;
using System.Linq;
using NPetrovich;

namespace IndexerLib.Lingva
{
    public static class KnownPersonSearcher
    {
        public class KnownPersonSearchResult
        {
            public KnownPerson KnownPerson { get; set; }
            public string Case { get; set; }
        }

        static Case GetCaseForSurname(KnownPerson person, string casedSurname)
        {
            if (casedSurname.Equals(person.NominativeSurname))
                return Case.Nominative;

            if (casedSurname.Equals(person.AccusativeSurname))
                return Case.Accusative;

            if (casedSurname.Equals(person.DativeSurname))
                return Case.Dative;

            if (casedSurname.Equals(person.GenitiveSurname))
                return Case.Genitive;

            if (casedSurname.Equals(person.InstrumentalSurname))
                return Case.Instrumental;

            if (casedSurname.Equals(person.PrepositionalSurname))
                return Case.Prepositional;

            return Case.Nominative;
        }

        static Case GetCaseForFirstName(KnownPerson person, string casedFirstName)
        {
            if (casedFirstName.Equals(person.NominativeName))
                return Case.Nominative;

            if (casedFirstName.Equals(person.AccusativeName))
                return Case.Accusative;

            if (casedFirstName.Equals(person.DativeName))
                return Case.Dative;

            if (casedFirstName.Equals(person.GenitiveName))
                return Case.Genitive;

            if (casedFirstName.Equals(person.InstrumentalName))
                return Case.Instrumental;

            if (casedFirstName.Equals(person.PrepositionalName))
                return Case.Prepositional;

            return Case.Nominative;
        }

        public static KnownPerson Search(Person person)
        {
            var fn = person.FirstName.ToLower();
            var sn = person.LastName.ToLower();

            var knownPersons =
                from kp in Context.Cached.KnownPersons
                where
                    (kp.NominativeName == fn && kp.NominativeSurname == sn) ||
                    (kp.AccusativeName == fn && kp.AccusativeSurname == sn) ||
                    (kp.DativeName == fn && kp.DativeSurname == sn) ||
                    (kp.GenitiveName == fn && kp.GenitiveSurname == sn) ||
                    (kp.InstrumentalName == fn && kp.InstrumentalSurname == sn) ||
                    (kp.PrepositionalName == fn && kp.PrepositionalSurname == sn)
                select kp;

            return knownPersons.FirstOrDefault();
        }

        public static IEnumerable<KnownPersonSearchResult> SearchByLastName(string lastName)
        {
            var persons =
                from kp in Context.Cached.KnownPersons
                where
                    kp.NominativeSurname == lastName ||
                    kp.AccusativeSurname == lastName ||
                    kp.DativeSurname == lastName ||
                    kp.GenitiveSurname == lastName ||
                    kp.InstrumentalSurname == lastName ||
                    kp.PrepositionalSurname == lastName
                select kp;

            return persons.Select(p => new KnownPersonSearchResult()
            {
                KnownPerson = p,
                Case = Enum.GetName(typeof (Case), GetCaseForSurname(p, lastName))
            }).ToArray();
        }
    }
}
