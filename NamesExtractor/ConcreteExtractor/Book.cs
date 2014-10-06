using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexerLib.ConcreteExtractor
{
    class Book : IBook
    {
        private string _text;

        private IEnumerable<IPersonReference> _references;

        public ITextSource Source { get; set; }

        public IEnumerable<IPage> Pages { get; set; }

        public int PagesCount { get { return Pages.Count(); } }

        public string Text
        {
            get
            {
                return _text ?? (_text = String.Join("\n", Pages.Select(p => p.Text).ToArray()));
            }
        }

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
