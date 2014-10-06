using System;
using System.Collections.Generic;
using System.IO;
using IndexerLib.Pdf;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace IndexerLib.Readers.Pdf
{
    public class PdfTextSource : ITextSource
    {
        private readonly string _file;
        private int _numberOfPages = -1;

        public int NumberOfPages
        {
            get
            {
                if (_numberOfPages == -1)
                {
                    using (var reader = new PdfReader(_file))
                    {
                        _numberOfPages = reader.NumberOfPages;
                    }
                }

                return _numberOfPages;
            }
        }

        public PdfTextSource(string file)
        {
            if(!File.Exists(file))
                throw new FileNotFoundException(
                    String.Format(@"File '{0}' not found", file)
                );

            _file = file;
        }

        public string GetText()
        {
            return String.Join("\n", GetPages());
        }

        public IEnumerable<string> GetPages()
        {
            string[] pages;
            
            using (var reader = new PdfReader(_file))
            {
                pages = new string[reader.NumberOfPages];
                for (var page = 1; page <= reader.NumberOfPages; page++)
                {
                    var strategy = new NoSpaceSimpleTextExtractionStrategy(new SimplePdfTextCleaner());
                    pages[page - 1] = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                }
            }

            return pages;
        }
    }
}
