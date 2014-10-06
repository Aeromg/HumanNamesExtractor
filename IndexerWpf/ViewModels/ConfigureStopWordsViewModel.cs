using IndexerLib.Persist;
using IndexerWpf.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace IndexerWpf.ViewModels
{
    public class ConfigureStopWordsViewModel : BaseViewModel
    {
        private  readonly SearchBox<StopWord> _searchBox;
        
        private string _testWord;

        private StopWord _selectedItem;

        public string TestWord
        {
            get
            {
                return _testWord;
            }
            set
            {
                if (_testWord != value)
                    _testWord = value;

                ReTest();
            }
        }

        public ObservableCollection<StopWord> Items
        {
            get
            {
                return _searchBox.Items;
            }
        }

        public StopWord SelectedItem 
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if(_selectedItem != value)
                    _selectedItem = value;

                EditItem();
            }
        }

        public Visibility TooManyResultsVisibility { get; private set; }

        public Visibility IsStopWord { get; private set; }

        public Visibility AddNewVisibility { get; private set; }

        public ICommand AddNew { get; private set; }

        public ConfigureStopWordsViewModel()
        {
            _searchBox = new SearchBox<StopWord>()
            {
                SearchFunc = ReTestImpl,
                NumberOfItemsLimit = 20
            };

            AddNew = new LambdaCommand()
            {
                ExecuteAction = AddNewImpl
            };

            TooManyResultsVisibility = IsStopWord = AddNewVisibility = Visibility.Collapsed;
        }

        public void ReTest()
        {
            _searchBox.Update();

            TooManyResultsVisibility = _searchBox.State == SearchBoxState.TooMuchResults ?
                Visibility.Visible : Visibility.Collapsed;

            IsStopWord = _searchBox.State == SearchBoxState.Ok ?
                Visibility.Visible : Visibility.Collapsed;

            AddNewVisibility = _searchBox.State == SearchBoxState.NoResults && TestWord.Length > 0 ?
                Visibility.Visible : Visibility.Collapsed;
        }

        IEnumerable<StopWord> ReTestImpl()
        {
            var stopWords = Context.Default.StopWords.ToArray();

            return
                from word in Context.Default.StopWords.ToArray()
                where word.Regex.IsMatch(TestWord.ToLower())
                select word;
        }

        void AddNewImpl()
        {
            var editorViewModel = new StopWordsEditorViewModel(TestWord);
            var editor = new StopWordsEditorPage(editorViewModel);

            MainWindow.Current.ShowModal(editor).ModalClosed = ReTest;
        }

        void EditItem()
        {
            if (SelectedItem == null)
                return;

            var editorViewModel = new StopWordsEditorViewModel(SelectedItem, TestWord);
            var editor = new StopWordsEditorPage(editorViewModel);

            MainWindow.Current.ShowModal(editor).ModalClosed = ReTest;
        }
    }
}
