using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IndexerLib;
using System.Collections.Generic;
using IndexerLib.Readers.Pdf;
using IndexerWpf.Views;
using Microsoft.Win32;
using Tests;
using IndexerLib.Lingva;

namespace IndexerWpf.ViewModels
{
    public class SearchResultViewModel : BaseViewModel
    {
        private readonly IBook _book;

        public SearchResultQuestionsViewModel QuestionsContext { get; set; }

        public SearchResultPersonsViewModel PersonsContext { get; set; }

        public ICommand GoBack { get; set; }

        public SearchResultViewModel()
            : this(TestBookBuilder.BuildBook())
        {
        }

        public SearchResultViewModel(IBook book)
        {
            _book = book;

            PersonsContext = new SearchResultPersonsViewModel(_book);
            QuestionsContext = new SearchResultQuestionsViewModel(_book);

            GoBack = new LambdaCommand()
            {
                ExecuteAction = Behavior.GoMain
            };
        }
    }

    public class SearchResultQuestionsViewModel : BaseViewModel
    {
        private readonly IBook _book;

        private readonly List<IPersonReference> _referencesBlackList;

        #region Persons selectors

        private IPersonReference _patternWordsSelectedItem;

        private IPersonReference _nominativePersonSelectedItem;

        private IPersonReference _selectedPersonReference;

        public ObservableCollection<IPersonReference> PatternWords { get; private set; }

        public ObservableCollection<IPersonReference> NominativePersons { get; private set; }

        public IPersonReference PatternWordsSelectedItem
        {
            get { return _patternWordsSelectedItem; }
            set
            {
                NominativePersonSelectedItem = null;

                if (value == _patternWordsSelectedItem)
                    return;

                _patternWordsSelectedItem = value;

                if(value == null)
                    PatternWordsSelectedIndex = -1;

                if (value != null)
                    SelectedPersonReference = value;
            }
        }

        public int PatternWordsSelectedIndex { get; set; }

        public IPersonReference NominativePersonSelectedItem
        {
            get { return _nominativePersonSelectedItem; }
            set
            {
                PatternWordsSelectedItem = null;

                if (value == _nominativePersonSelectedItem)
                    return;

                _nominativePersonSelectedItem = value;

                if (value == null)
                    NominativePersonSelectedIndex = -1;

                if (value != null)
                    SelectedPersonReference = value;
            }
        }

        public int NominativePersonSelectedIndex { get; set; }

        public IPersonReference SelectedPersonReference
        {
            get { return _selectedPersonReference; }
            set
            {
                if(value == _selectedPersonReference)
                    return;

                _selectedPersonReference = value;

                UpdateReferences(value == null ? "" : value.Person.FullName);

                SolutionsVisibility = value == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility ErrorsVisibility { get; set; }

        public Visibility NominativesVisibility { get; set; }

        #endregion

        #region Pages selector

        private IPersonReference _referencesSelectedItem;

        public ObservableCollection<IPersonReference> References { get; set; }

        public IPersonReference ReferencesSelectedItem
        {
            get { return _referencesSelectedItem; }
            set
            {
                if (value == _referencesSelectedItem)
                    return;

                if (value == null)
                    ReferencesSelectedIndex = -1;

                _referencesSelectedItem = value;

                if(value != null)
                    UpdateLookup(value);
            }
        }

        public int ReferencesSelectedIndex { get; set; }

        void UpdateReferences(string personFullName)
        {
            var references =
                (from reference in _book.References
                 where reference.Person.FullName == personFullName
                 orderby reference.Page.PageNumber ascending 
                 select reference).ToArray();

            References.Clear();

            foreach (var reference in references)
                References.Add(reference);

            ReferencesSelectedIndex = References.Count > 0 ? 0 : -1;
        }

        #endregion

        #region Lookup text

        public string Lookup { get; set; }

        public void UpdateLookup(IPersonReference reference)
        {
            if (reference == null)
            {
                Lookup = "";
                return;
            }

            Lookup = reference.Token.Lookup;
        }

        #endregion

        #region Base behavior

        public Visibility TabVisibility
        {
            get { return IsQuestionsExists ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility SolutionsVisibility { get; set; }

        public ICommand SolveCommit { get; set; }

        public ICommand SolveRemove { get; set; }

        public ICommand CommitAnswers { get; set; }

        public bool IsQuestionsExists
        {
            get { return PatternWords.Any() || NominativePersons.Any(); }
        }

        public SearchResultQuestionsViewModel(IBook book)
        {
            _book = book;
            _referencesBlackList = new List<IPersonReference>();

            PatternWords = new ObservableCollection<IPersonReference>(
                GetUniquePersonReferences(NameProbability.MatchPattern)
            );

            NominativePersons = new ObservableCollection<IPersonReference>(
                GetUniquePersonReferences(NameProbability.NominativeName)
            );

            References = new ObservableCollection<IPersonReference>();

            PatternWordsSelectedIndex = -1;
            NominativePersonSelectedIndex = -1;

            SolutionsVisibility = Visibility.Collapsed;

            SolveCommit = new LambdaCommand()
            {
                ExecuteAction = SolveCommitImpl
            };

            SolveRemove = new LambdaCommand()
            {
                ExecuteAction = SolveRemoveImpl
            };

            CommitAnswers = new LambdaCommand()
            {
                ExecuteAction = CommitAnswersImpl
            };

            NominativePersons.CollectionChanged += (sender, args) => UpdatePersonHeaders();
            PatternWords.CollectionChanged += (sender, args) => UpdatePersonHeaders();

            UpdatePersonHeaders();

            // Lookup = @"Тут какой-то волшебный текст";
        }

        IEnumerable<IPersonReference> GetUniquePersonReferences(NameProbability probability)
        {
            return
                (from r in _book.References
                where r.Probability == probability
                group r by r.Person.FullName into u
                select u.First()).OrderBy(u => u.Person.FullName);
        }

        void SolveCommitImpl()
        {
            var item = ReferencesSelectedItem;

            if (item == null)
                return;

            var isMale = KnownNamesSearcher.CheckIfMale(item.Person.FirstName);

            var editor =
                new KnownPersonEditorPage(new KnownPersonEditorViewModel(item.Person.FirstName, item.Person.LastName, isMale));

            MainWindow.Current.ShowModal(editor).ModalClosed = () =>
            {
                if (Essentials.SearchKnownPersonsByText(item.Person.FullName).Any())
                    RemoveReference(item);
            };
        }

        void SolveRemoveImpl()
        {
            var item = ReferencesSelectedItem;

            if (item == null)
                return;

            var editor = new StopWordsEditorPage(new StopWordsEditorViewModel(item.Token.Text));

            MainWindow.Current.ShowModal(editor).ModalClosed = () =>
            {
                RemoveReference(item);
                _referencesBlackList.Add(item);
            };

        }

        void CommitAnswersImpl()
        {
            Behavior.ProcessReading(_book.Source);
        }

        void RemoveReference(IPersonReference reference)
        {
            References.Remove(reference);
            PatternWords.Remove(reference);
            NominativePersons.Remove(reference);

            ReferencesSelectedItem = null;
            UpdateLookup(null);
        }

        void UpdatePersonHeaders()
        {
            ErrorsVisibility = PatternWords.Any() ? Visibility.Visible : Visibility.Collapsed;
            NominativesVisibility = NominativePersons.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }

    public class SearchResultPersonsViewModel : BaseViewModel
    {
        private readonly IBook _book;

        #region Persons selectors

        private IPersonReference _questionsSelectedItem;

        public ObservableCollection<IPersonReference> Questions { get; private set; }

        public int QuestionsSelectedIndex { get; set; }

        public IPersonReference QuestionsSelectedItem
        {
            get { return _questionsSelectedItem; }
            set
            {
                if (value == _questionsSelectedItem)
                    return;

                _questionsSelectedItem = value;

                if (value == null)
                    QuestionsSelectedIndex = -1;

                UpdateReferences(_questionsSelectedItem == null ? "" : _questionsSelectedItem.Person.FullName);
            }
        }

        #endregion

        #region Pages selector

        private IPersonReference _referencesSelectedItem;

        public ObservableCollection<IPersonReference> References { get; set; }

        public IPersonReference ReferencesSelectedItem
        {
            get { return _referencesSelectedItem; }
            set
            {
                if (value == _referencesSelectedItem)
                    return;

                if (value == null)
                    ReferencesSelectedIndex = -1;

                _referencesSelectedItem = value;

                if (value != null)
                    UpdateLookup(value);
            }
        }

        public int ReferencesSelectedIndex { get; set; }

        void UpdateReferences(string personFullName)
        {
            var references =
                (from reference in _book.References
                 where reference.Person.FullName == personFullName
                 orderby reference.Page.PageNumber ascending
                 select reference).ToArray();

            References.Clear();

            foreach (var reference in references)
                References.Add(reference);

            ReferencesSelectedIndex = References.Count > 0 ? 0 : -1;
        }

        #endregion

        #region Lookup text

        public string Lookup { get; set; }

        public void UpdateLookup(IPersonReference reference)
        {
            Lookup = reference.Token.Lookup;
        }

        #endregion

        #region Base behavior

        public ICommand Ok { get; set; }

        public SearchResultPersonsViewModel(IBook book)
        {
            _book = book;

            Questions = new ObservableCollection<IPersonReference>(
                GetUniquePersonReferences(NameProbability.KnownPerson)
            );

            References = new ObservableCollection<IPersonReference>();

            QuestionsSelectedIndex = -1;

            Ok = new LambdaCommand()
            {
                ExecuteAction = ExportBook
            };

            // Lookup = @"Тут какой-то волшебный текст";
        }

        IEnumerable<IPersonReference> GetUniquePersonReferences(NameProbability probability)
        {
            return
                (from r in _book.References
                where r.Probability == probability
                group r by r.Person.FullName into u
                select u.First()).OrderBy(u => u.Person.FullName);
        }

        void ExportBook()
        {
            var dialog = new SaveFileDialog()
            {
                // Filter = "PDF Files (.pdf)|*.pdf|Text Files (.txt)|*.txt|All Files (*.*)|*.*",
                Filter = "TXT File (.txt)|*.txt",
                FilterIndex = 1
            };

            var userClickedOk = dialog.ShowDialog();

            if (userClickedOk != true) return;

            var exporter = new TextFileResultExporter(dialog.FileName);
            exporter.ExportPersons(_book);
        }

        #endregion
    }
}
