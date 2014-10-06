using System;
using System.Collections.Generic;
using System.IO;

namespace IndexerLib.Readers.PlainText
{
    public class PlainTextSource : ITextSource
    {
        private readonly string _file;
        public int NumberOfPages { get; private set; }

        public PlainTextSource(string file, int nuberOfPages)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(
                    String.Format(@"File '{0}' not found", file)
                );

            _file = file;
            NumberOfPages = new FileInfo(file).Length > 0 ? 1 : 0;
        }

        public string GetText()
        {
            return File.ReadAllText(_file);
        }

        public IEnumerable<string> GetPages()
        {
            return new[] {GetText()};
        }
    }
}
