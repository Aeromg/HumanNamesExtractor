using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IndexerLib;
using IndexerLib.Lingva;
using IndexerLib.Persist;
using IndexerLib.Readers.Pdf;
using Tests;

namespace IndexerWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }

        public static long RetrieveLinkerTimestampVersion()
        {
            return RetrieveLinkerTimestamp().Ticks / 10000000;
        }

        public App()
            : base()
        {
            //Tools.MergeKnownNamesWithKnownPersons();
            /*var files = Directory.EnumerateFiles(@"c:\Temp\learn\", @"*.pdf", SearchOption.AllDirectories).ToArray();

            foreach (var file in files)
                LearnIndexPageFile(file);*/
        }

        static void LearnIndexedPaper(string fileName)
        {
            var book = Essentials.ReadBook(new PdfTextSource(fileName));
            if(book.PagesCount < 3)
                throw new Exception();

            var indexPersons = book.Pages.
                First(p => p.PageNumber == 2).
                References.Where(r => r.Probability == NameProbability.NominativeName).
                Select(r => r.Person).
                Distinct().
                ToArray();

            foreach (var person in indexPersons)
                InsertKnownPerson(person);

            if (indexPersons.Length > 0)
            {
                Context.Default.SaveChanges();
                Context.Cached.Reset();
            }
        }

        static void LearnIndexPageFile(string fileName)
        {
            var book = Essentials.ReadBook(new PdfTextSource(fileName));
            if (book.PagesCount != 1)
                return;

            var indexPersons = book.Pages.First().
                References.Where(r => r.Probability == NameProbability.NominativeName).
                Select(r => r.Person).
                Distinct().
                ToArray();

            foreach (var person in indexPersons)
                InsertKnownPerson(person);

            if (indexPersons.Length > 0)
            {
                Context.Default.SaveChanges();
                Context.Cached.Reset();
            }
        }

        static void InsertKnownPerson(Person person)
        {
            var isMale = KnownNamesSearcher.CheckIfMale(person.FirstName);
            var knownPerson = Essentials.CreateKnownPerson(person.FirstName, person.LastName, isMale);
            Context.Default.KnownPersons.Add(knownPerson);
        }
    }
}
