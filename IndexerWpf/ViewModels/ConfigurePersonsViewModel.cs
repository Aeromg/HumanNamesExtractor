using IndexerLib;
using System.Collections.Generic;
using IndexerLib.Lingva;
using IndexerLib.Persist;
using System.Collections.ObjectModel;
using System.Windows;
using IndexerWpf.Views;

namespace IndexerWpf.ViewModels
{
    public class ConfigurePersonsViewModel : BaseViewModel
    {
        private string _searchText;

        private SearchBox<KnownPerson> _searchBox { get; set; }

        private KnownPerson _selectedItem;

        [NoMagic]
        public string SearchText 
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");
                UpdateSearchResults();
            }
        }

        public LambdaCommand AddNew { get; set; }

        public Visibility AddNewVisibility { get; set; }

        public Visibility TooManyResultsVisibility { get; set; }

        public Visibility IsNotEnoughtForName { get; set; }

        public ObservableCollection<KnownPerson> Items 
        {
            get
            {
                return _searchBox.Items;
            }
        }

        public KnownPerson SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                EditItem();
            }
        }

        public ConfigurePersonsViewModel()
        {
            AddNew = new LambdaCommand()
            {
                CanExecuteFunc = CanAddNewImpl,
                ExecuteAction = AddNewImpl
            };

            _searchBox = new SearchBox<KnownPerson>()
            {
                NumberOfItemsLimit = 24,
                SearchFunc = SearchFuncImpl
            };

            IsNotEnoughtForName = AddNewVisibility = TooManyResultsVisibility = Visibility.Collapsed;

            PropertyChanged += (prop, arg) =>
            {
                var a = arg.PropertyName;
            };
        }

        bool CanAddNewImpl()
        {
            Person person;
            if (!Person.TryParse(SearchText, out person))
                return false;

            return KnownPersonSearcher.Search(person) == null;
        }

        void AddNewImpl()
        {
            var person = Person.Parse(SearchText);
            var isMale = KnownNamesSearcher.CheckIfMale(person.FirstName);
            var editor = new KnownPersonEditorPage(new KnownPersonEditorViewModel(person.FirstName, person.LastName, isMale));
            MainWindow.Current.ShowModal(editor).ModalClosed = UpdateSearchResults;
        }

        bool IsValidName()
        {
            Person person;
            return Person.TryParse(SearchText, out person);
        }

        IEnumerable<KnownPerson> SearchFuncImpl()
        {
            Person person;
            return Essentials.SearchKnownPersonsByText(!Person.TryParse(SearchText, out person) ? SearchText : person.FullName);
        }

        void UpdateSearchResults()
        {
            App.Current.Dispatcher.Invoke(() => _searchBox.Update());

            IsNotEnoughtForName = _searchBox.State == SearchBoxState.NoResults && !IsValidName() && SearchText.Trim().Length > 0 ?
                Visibility.Visible : Visibility.Collapsed;

            AddNewVisibility = _searchBox.State == SearchBoxState.NoResults && IsValidName() ? 
                Visibility.Visible : Visibility.Collapsed;

            TooManyResultsVisibility = _searchBox.State == SearchBoxState.TooMuchResults ?
                Visibility.Visible : Visibility.Collapsed;

            AddNew.OnCanExecuteChanged();
        }

        void EditItem()
        {
            if (SelectedItem == null)
                return;

            var editor = new KnownPersonEditorPage(new KnownPersonEditorViewModel(_selectedItem));
            MainWindow.Current.ShowModal(editor).ModalClosed = UpdateSearchResults;
            SelectedItem = null;
        }
    }
}
