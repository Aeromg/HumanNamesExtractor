using IndexerLib.Persist;
using System.Text.RegularExpressions;
using System.Windows;

namespace IndexerWpf.ViewModels
{
    public class StopWordsEditorViewModel : BaseViewModel
    {
        public delegate void StopWordChangedDelegate(string stopWord);

        private string _stopWordText;

        private StopWord _stopWordEntity;

        public string StopWordText
        {
            get
            {
                return _stopWordText;
            }
            set
            {
                if (_stopWordText != value)
                    _stopWordText = value;

                OnStopWordChanged();
            }
        }

        public string TestText { get; set; }

        public LambdaCommand AddNew { get; private set; }

        public LambdaCommand Remove { get; private set; }

        public LambdaCommand GoBack { get; private set; }

        public Visibility RemoveVisibility { get; private set; }

        public event StopWordChangedDelegate StopWordChanged;

        public StopWordsEditorViewModel() 
        {
            AddNew = new LambdaCommand()
            {
                ExecuteAction = AddNewImpl,
                CanExecuteFunc = CanAdd
            };

            Remove = new LambdaCommand()
            {
                ExecuteAction = RemoveImpl
            };

            GoBack = new LambdaCommand()
            {
                ExecuteAction = MainWindow.Current.CloseModal
            };

            RemoveVisibility = Visibility.Collapsed;
        }

        public StopWordsEditorViewModel(string testText)
            : this()
        {
            TestText = testText;

            Fill();
        }

        public StopWordsEditorViewModel(StopWord stopWord, string testText)
            : this()
        {
            _stopWordEntity = stopWord;
            TestText = testText;

            RemoveVisibility = Visibility.Visible;

            Fill();
        }

        void Fill()
        {
            if (_stopWordEntity != null)
                StopWordText = _stopWordEntity.Pattern;
            else
                StopWordText = TestText;
        }

        void AddNewImpl()
        {
            var stopWord = _stopWordEntity ?? new StopWord();

            stopWord.Pattern = StopWordText;

            if (stopWord.Id == 0)
                Context.Default.StopWords.Add(stopWord);

            Context.Default.SaveChanges();

            GoBack.Execute(new { });
        }

        void RemoveImpl()
        {
            Context.Default.StopWords.Remove(_stopWordEntity);
            Context.Default.SaveChanges();

            GoBack.Execute(new { });
        }

        void OnStopWordChanged()
        {
            var handler = StopWordChanged;

            if (handler != null)
                handler(StopWordText);

            AddNew.OnCanExecuteChanged();
        }

        bool CanAdd()
        {
            if (StopWordText.Trim().Length == 0)
                return false;

            try
            {
                new Regex(StopWordText);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
