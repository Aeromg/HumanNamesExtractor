using System.Collections.Generic;
using System.Linq;
using IndexerLib;

namespace Tests
{
    class TestBook : IBook
    {
        private IEnumerable<IPersonReference> _references;

        public ITextSource Source { get; set; }

        public IEnumerable<IPage> Pages { get; set; }

        public int PagesCount { get { return Pages.Count(); } }

        public string Text { get { return Source.GetText(); } }

        public IEnumerable<IPersonReference> References
        {
            get
            {
                return _references ?? (_references = CollectReferences());
            }
        }

        IEnumerable<IPersonReference> CollectReferences()
        {
            var references = new List<IPersonReference>();

            foreach (var page in Pages)
                references.AddRange(page.References);

            return references;
        }
    }
}
