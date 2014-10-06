using System.Collections.Generic;

namespace IndexerLib
{
    public interface IPage
    {
        int PageNumber { get; }
        string Text { get; }
        IEnumerable<IPersonReference> References { get; }
    }
}
