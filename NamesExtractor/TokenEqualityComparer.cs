using System.Collections.Generic;

namespace IndexerLib
{
    class TokenEqualityComparer : IEqualityComparer<Token>
    {
        public bool Equals(Token x, Token y)
        {
            var textX = x == null ? "" : x.Text;
            var textY = y == null ? "" : y.Text;

            return x == y;
        }

        public int GetHashCode(Token obj)
        {
            return obj.GetHashCode();
        }
    }
}
