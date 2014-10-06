using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using IndexerWpf.ViewModels;
using Tests;

namespace IndexerWpf.Views
{
    /// <summary>
    /// Interaction logic for SearchResultPage.xaml
    /// </summary>
    public partial class SearchResultPage : Page
    {
        private readonly SearchResultViewModel _viewModel;

        private static readonly DependencyObjectMarker Marker = new DependencyObjectMarker()
        {
            Background = Brushes.OrangeRed,
            Foreground = Brushes.White
        };

        public SearchResultPage() : this(new SearchResultViewModel(TestBookBuilder.BuildBook())) { }

        public SearchResultPage(SearchResultViewModel viewModel)
        {
            InitializeComponent();
            
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.PersonsContext.PropertyChanged += PersonsContextPropertyChanged;
            _viewModel.QuestionsContext.PropertyChanged += QuestionsContextPropertyChanged;

            Tabs.SelectedIndex = _viewModel.QuestionsContext.IsQuestionsExists ? 0 : 1;
        }

        void PersonsContextPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Lookup")
            {
                LookupTextBlock.Text = _viewModel.PersonsContext.Lookup;
                Marker.TryHighlight(LookupTextBlock, _viewModel.PersonsContext.ReferencesSelectedItem.Token.Text);
            }
        }

        void QuestionsContextPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Lookup")
            {
                LookupQuestionTextBlock.Text = _viewModel.QuestionsContext.Lookup;
                Marker.TryHighlight(LookupQuestionTextBlock, _viewModel.QuestionsContext.SelectedPersonReference.Token.Text);
            }
        }
    }
}
