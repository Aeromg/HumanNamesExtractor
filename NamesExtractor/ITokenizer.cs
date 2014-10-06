using System.Collections.Generic;

namespace IndexerLib
{
    public interface ITokenizer
    {
        IEnumerable<Token> GetTokens(string text);
    }
}
