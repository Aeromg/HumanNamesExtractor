using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IndexerWpf
{
    public class DependencyObjectMarker
    {
        public Brush Background { get; set; }

        public Brush Foreground { get; set; }

        public bool TryHighlight(DependencyObject obj, string pattern, bool ignoreCase = true)
        {
            try
            {
                var options = RegexOptions.Multiline | (ignoreCase ? RegexOptions.IgnoreCase : 0);
                var regex = new Regex(pattern, options);

                Highlight(obj, regex);

                return true;
            }
            catch
            {
                Highlight(obj, new Regex(@""));

                return false;
            }
        }

        public void Highlight(DependencyObject obj, Regex regex)
        {
            if (obj == null)
                return;

            if (obj is TextBlock)
            {
                HighlightTextBlock(obj as TextBlock, regex);
            }
            else
            {
                var numberOfChilds = VisualTreeHelper.GetChildrenCount(obj);
                for (int childIndex = 0; childIndex < numberOfChilds; childIndex++)
                    Highlight(VisualTreeHelper.GetChild(obj, childIndex), regex);
            }
        }

        void HighlightTextBlock(TextBlock textBlock, Regex regex)
        {
            var clearText = textBlock.Text;

            if (clearText.Length == 0)
                return;

            textBlock.Inlines.Clear();

            var matches = regex.Matches(clearText).Cast<Match>().Where(m => m.Length > 0).ToArray();

            int lastMatchEndIndex = 0;
            int resultTextLength = 0;

            foreach (var match in matches)
            {
                if (resultTextLength < match.Index)
                    textBlock.Inlines.Add(clearText.Substring(lastMatchEndIndex, match.Index - lastMatchEndIndex));

                textBlock.Inlines.Add(new Run(match.Value)
                {
                    Background = this.Background,
                    Foreground = this.Foreground
                });

                resultTextLength += match.Value.Length;
                lastMatchEndIndex = match.Index + match.Length;
            }

            if (lastMatchEndIndex <= clearText.Length)
                textBlock.Inlines.Add(clearText.Substring(lastMatchEndIndex, clearText.Length - lastMatchEndIndex));

            return;
        }
    }
}
