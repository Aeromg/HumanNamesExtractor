using IndexerLib.RegexExtension;
using IndexerWpf.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace IndexerWpf.Views
{
    /// <summary>
    /// Interaction logic for StopWordsEditorPage.xaml
    /// </summary>
    public partial class StopWordsEditorPage : Page
    {
        StopWordsEditorViewModel _viewModel;
        DependencyObjectMarker _marker;

        public StopWordsEditorPage()
        {
            _marker = new DependencyObjectMarker()
            {
                Background = Brushes.Red,
                Foreground = Brushes.White
            };

            InitializeComponent();
        }

        public StopWordsEditorPage(StopWordsEditorViewModel viewModel) : this()
        {
            _viewModel = viewModel;
            _viewModel.StopWordChanged += StopWordChanged;

            DataContext = _viewModel;
        }

        void StopWordChanged(string stopWord)
        {
            stopWord = RegexFunctionExecutor.ExecuteExpression(stopWord);

            var marked = _marker.TryHighlight(TextSandbox, stopWord);
            /*
            StopWordTextBox.Background = marked ?
                Brushes.Transparent :
                Brushes.Red; */
        }
    }
}
