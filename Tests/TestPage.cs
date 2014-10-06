using System.Collections.Generic;
using IndexerLib;

namespace Tests
{
    class TestPage : IPage
    {
        public int PageNumber { get; set; }

        public string Text { get; set; }

        public IEnumerable<IPersonReference> References { get; set; }
    }
}
