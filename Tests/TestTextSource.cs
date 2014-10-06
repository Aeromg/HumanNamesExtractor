using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexerLib;

namespace Tests
{
    class TestTextSource : ITextSource
    {
        public IEnumerable<string> Pages { get; set; }

        public int NumberOfPages { get { return Pages.Count(); } }

        public string GetText()
        {
            return String.Join("\n", Pages);
        }

        public IEnumerable<string> GetPages()
        {
            return Pages.ToArray();
        }
    }
}
