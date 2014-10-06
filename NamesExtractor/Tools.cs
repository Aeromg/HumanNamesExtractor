using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexerLib.Lingva;
using IndexerLib.Persist;
using NPetrovich;

namespace IndexerLib
{
    public class Tools
    {
        class KnownNamesImporter
        {
            private readonly MyOwnPetrovich _petrovich = new MyOwnPetrovich();

            IEnumerable<string> ReadNames(string fileName)
            {
                var names = new HashSet<string>();
                using (var input = File.OpenText(fileName))
                {
                    string name = String.Empty;
                    while (!input.EndOfStream)
                    {
                        name = input.ReadLine();
                        if (!names.Contains(name))
                            names.Add(name);
                    }
                }

                return names;
            }

            public void Import(string malesFileName, string femalesFileName)
            {
                var males = ReadNames(malesFileName);
                
                var femalesAll = ReadNames(femalesFileName);
                var females = femalesAll.Where(name => !males.Contains(name)).ToArray();

                IEnumerable<KnownName> malesKnowNames = null;
                IEnumerable<KnownName> femalesKnownNames = null;

                var malesTask = new Task(() => malesKnowNames = males.Select(n => CreateKnownName(n, true)));
                var femalesTask = new Task(() => femalesKnownNames = females.Select(n => CreateKnownName(n, false)));

                var namesTask = new Task(() =>
                {
                    malesTask.Start();
                    femalesTask.Start();

                    malesTask.Wait();
                    femalesTask.Wait();
                });

                namesTask.Start();
                namesTask.Wait();

                var names = malesKnowNames.Concat(femalesKnownNames).ToArray();

                Context.Default.KnownNames.AddRange(names);
                //Context.Default.SaveChanges();
            }

            KnownName CreateKnownName(string name, bool isMale)
            {
                _petrovich.SetGender(isMale ? Gender.Male : Gender.Female);

                _petrovich.SetNominative(firstName: name);

                var knownName = new KnownName()
                {
                    Accusative = _petrovich.InflectFirstNameTo(Case.Accusative),
                    Dative = _petrovich.InflectFirstNameTo(Case.Dative),
                    Genitive = _petrovich.InflectFirstNameTo(Case.Genitive),
                    Instrumental = _petrovich.InflectFirstNameTo(Case.Instrumental),
                    Nominative = _petrovich.InflectFirstNameTo(Case.Nominative),
                    Prepositional = _petrovich.InflectFirstNameTo(Case.Prepositional),
                    IsMale = isMale
                };

                return knownName;
            }
        }

        public static void ImportKnownNames(string malesFileName, string femalesFileName)
        {
            var importer = new KnownNamesImporter();
            importer.Import(malesFileName, femalesFileName);
        }

        public static void MergeKnownNamesWithKnownPersons()
        {
            var addedNames = new HashSet<string>();
            var knownPersons = Context.Default.KnownPersons;
            foreach (var knownPerson in knownPersons)
            {
                if (addedNames.Contains(knownPerson.NominativeName))
                    continue;

                MergeKnownNameWithKnownPerson(knownPerson);

                addedNames.Add(knownPerson.NominativeName);
            }

            Context.Default.SaveChanges();
        }

        public static void MergeKnownNameWithKnownPerson(KnownPerson person)
        {
            if (Context.Default.KnownNames.Any(n => n.Nominative == person.NominativeName))
                return;

            var knownName = new KnownName()
            {
                Nominative = person.NominativeName,
                Accusative = person.AccusativeName,
                Dative = person.DativeName,
                Genitive = person.GenitiveName,
                Instrumental = person.InstrumentalName,
                Prepositional = person.PrepositionalName,
                IsMale = true
            };

            Context.Default.KnownNames.Add(knownName);
        }
    }
}
