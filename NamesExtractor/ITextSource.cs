using System.Collections.Generic;

namespace IndexerLib
{
    public interface ITextSource
    {
        int NumberOfPages { get; }
        string GetText();
        IEnumerable<string> GetPages();
    }
}
