using System.Collections.Generic;

namespace IndexerLib.ConcreteExtractor
{
    class Page : IPage
    {
        public int PageNumber { get; set; }

        public string Text { get; set; }

        public IEnumerable<IPersonReference> References { get; set; }
    }
}
