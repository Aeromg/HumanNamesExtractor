using IndexerLib.Lingva;
using IndexerLib.Persist;
using NPetrovich;
using System.Collections.Generic;
using System.Linq;

namespace IndexerLib
{
    public static class Essentials
    {
        public static IBook ReadBook(ITextSource source)
        {
            Context.Cached.Warm();

            var book = new ConcreteExtractor.Book()
            {
                Pages = GetPages(source),
                Source = source
            };
            
            return book;
        }

        public static KnownPerson CreateKnownPerson(string firstName, string lastName, bool isMale = true)
        {
            firstName = firstName.Trim().ToLower();
            lastName = lastName.Trim().ToLower();

            var petrovich = new MyOwnPetrovich();
            petrovich.SetGender(isMale ? Gender.Male : Gender.Female);
            petrovich.SetNominative(firstName, "", lastName);

            var knownPerson = new KnownPerson()
            {
                NominativeName = firstName,
                NominativeSurname = lastName,

                AccusativeName = petrovich.InflectFirstNameTo(Case.Accusative),
                AccusativeSurname = petrovich.InflectLastNameTo(Case.Accusative),

                DativeName = petrovich.InflectFirstNameTo(Case.Dative),
                DativeSurname = petrovich.InflectLastNameTo(Case.Dative),

                GenitiveName = petrovich.InflectFirstNameTo(Case.Genitive),
                GenitiveSurname = petrovich.InflectLastNameTo(Case.Genitive),

                InstrumentalName = petrovich.InflectFirstNameTo(Case.Instrumental),
                InstrumentalSurname = petrovich.InflectLastNameTo(Case.Instrumental),

                PrepositionalName = petrovich.InflectFirstNameTo(Case.Prepositional),
                PrepositionalSurname = petrovich.InflectLastNameTo(Case.Prepositional)
            };

            return knownPerson;
        }

        public static KnownName CreateKnownName(string name, bool isMale)
        {
            var petrovich = new MyOwnPetrovich();
            petrovich.SetGender(isMale ? Gender.Male : Gender.Female);

            petrovich.SetNominative(firstName: name);

            var knownName = new KnownName()
            {
                Accusative = petrovich.InflectFirstNameTo(Case.Accusative),
                Dative = petrovich.InflectFirstNameTo(Case.Dative),
                Genitive = petrovich.InflectFirstNameTo(Case.Genitive),
                Instrumental = petrovich.InflectFirstNameTo(Case.Instrumental),
                Nominative = petrovich.InflectFirstNameTo(Case.Nominative),
                Prepositional = petrovich.InflectFirstNameTo(Case.Prepositional),
                IsMale = isMale
            };

            Context.Default.KnownNames.Add(knownName);
            Context.Default.SaveChanges();

            return knownName;
        }

        public static IEnumerable<KnownPerson> SearchKnownPersonsByText(string searchString)
        {
            return KnownPerson.SearchByText(searchString);
        }

        static IEnumerable<IPage> GetPages(ITextSource source)
        {
            var pagesText = source.GetPages().ToArray();
            var numberOfPages = pagesText.Length;
            var pages = new IPage[numberOfPages];

            var idx = Enumerable.Range(0, numberOfPages);

            idx.AsParallel().ForAll((i) =>
            {
                pages[i] = GetPage(pagesText[i], i + 1);
            });

            /*for (var page = 0; page < numberOfPages; page++)
            {
                pages[page] = GetPage(pagesText[page], page + 1);
            }*/

            return pages;
        }

        static IPage GetPage(string text, int pageNumber)
        {
            var page = new ConcreteExtractor.Page() {
                Text = text,
                PageNumber = pageNumber
            };

            page.References = GetReferences(page);

            return page;
        }

        static IEnumerable<IPersonReference> GetReferences(IPage page)
        {
            var names = new NamesExtractor(new NamesTokenizer()).ExtractNames(page.Text);

            return
                from name in names
                select new ConcreteExtractor.PersonReference()
                {
                    Person = name.Person,
                    Probability = name.Probability,
                    Token = name.Token,
                    Page = page
                };
        }
    }
}
