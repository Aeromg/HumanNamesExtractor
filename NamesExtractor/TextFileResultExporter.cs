using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexerLib
{
    public class TextFileResultExporter : IResultExporter
    {
        class BasicTextFileResultFormatter : ITextFileResultExporterFormatter
        {
            public string Format(Person person, IBook book)
            {
                var pages =
                    from reference in book.References
                    where reference.Person.FullName == person.FullName
                    select reference.Page.PageNumber;

                var personPart = person.FullName;
                var pagesPart = String.Join(", ", pages);

                return String.Format("{0}\t{1}", personPart, pagesPart);
            }

            public string Format(IPage page)
            {
                var pagePart = page.PageNumber;
                var personsPart = String.Join(@", ", page.References.Select(r => r.Person.FullName));

                return String.Format(@"{0}\t{1}", pagePart, personsPart);
            }
        }

        private readonly string _fileName;
        private readonly ITextFileResultExporterFormatter _formatter;

        public TextFileResultExporter(string fileName, ITextFileResultExporterFormatter formatter = null)
        {
            _fileName = fileName;
            _formatter = formatter ?? new BasicTextFileResultFormatter();
        }

        public void ExportPersons(IBook book)
        {
            using (var output = File.CreateText(_fileName))
            {
                foreach (var reference in book.References)
                    output.WriteLine(_formatter.Format(reference.Person, book));

                output.Flush();
            }
        }

        public void ExportPages(IBook book)
        {
            using (var output = File.CreateText(_fileName))
            {
                foreach (var page in book.Pages)
                    output.WriteLine(_formatter.Format(page));

                output.Flush();
            }
        }
    }

    public interface ITextFileResultExporterFormatter
    {
        string Format(Person person, IBook book);
        string Format(IPage page);
    }
}
