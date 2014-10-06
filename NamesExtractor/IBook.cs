using System.Collections.Generic;

namespace IndexerLib
{
    public interface IBook
    {
        ITextSource Source { get; }
        IEnumerable<IPage> Pages { get; }
        int PagesCount { get; }
        string Text { get; }
        IEnumerable<IPersonReference> References { get; }
    }
}
