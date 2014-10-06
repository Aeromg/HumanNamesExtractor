using System;
using System.Text;
using iTextSharp.text.pdf.parser;

namespace IndexerLib.Pdf
{
    class NoSpaceSimpleTextExtractionStrategy : ITextExtractionStrategy
    {
        private readonly ITextCleaner _cleaner;
        private Vector _lastStart;
        private Vector _lastEnd;

        private readonly StringBuilder _result = new StringBuilder();

        public NoSpaceSimpleTextExtractionStrategy(ITextCleaner cleaner = null)
        {
            _cleaner = cleaner;
        }

        public virtual void BeginTextBlock()
        {
        }

        public virtual void EndTextBlock()
        {
        }

        public virtual String GetResultantText()
        {
            var result = _result.ToString();
            return _cleaner == null ? result : _cleaner.Clean(result);
        }

        public virtual void RenderText(TextRenderInfo renderInfo)
        {
            var firstRender = _result.Length == 0;
            var hardReturn = false;

            var segment = renderInfo.GetBaseline();
            var start = segment.GetStartPoint();
            var end = segment.GetEndPoint();

            const float sameLineThreshold = 1f; // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being

            if (!firstRender)
            {
                var x0 = start;
                var x1 = _lastStart;
                var x2 = _lastEnd;

                // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                var dist = (x2.Subtract(x1)).Cross((x1.Subtract(x0))).LengthSquared / x2.Subtract(x1).LengthSquared;

                if (dist > sameLineThreshold)
                    hardReturn = true;
            }

            if (hardReturn)
                _result.Append('\n');

            _result.Append(renderInfo.GetText());

            _lastStart = start;
            _lastEnd = end;
        }

        public virtual void RenderImage(ImageRenderInfo renderInfo) { }
    }
}