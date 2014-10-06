using IndexerLib;
using IndexerLib.Lingva;
using IndexerLib.Persist;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace IndexerWpf.ViewModels
{
    public class KnownPersonEditorViewModel : BaseViewModel
    {
        private readonly bool _isNew;
        
        private KnownPerson _person;

        private bool _isBoy;

        public string NominativeFullName { get; set; }

        public string NominativeName { get; set; }
        public string NominativeSurname { get; set; }

        public string GenitiveName { get; set; }
        public string GenitiveSurname { get; set; }

        public string DativeName { get; set; }
        public string DativeSurname { get; set; }

        public string AccusativeName { get; set; }
        public string AccusativeSurname { get; set; }

        public string InstrumentalName { get; set; }
        public string InstrumentalSurname { get; set; }

        public string PrepositionalName { get; set; }
        public string PrepositionalSurname { get; set; }

        public bool IsBoy
        {
            get
            {
                return _isBoy;
            }
            set
            {
                _isBoy = value;
                if(!_isNew)
                    return;

                _person = Essentials.CreateKnownPerson(NominativeName, NominativeSurname, IsBoy);
                Fill();
            }
        }

        public Visibility IsBoyVisibility { get { return _isNew ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility IsBadNameVisibility { get; set; }

        public Visibility IsAutoInflectedVisibility { get; set; }

        public Visibility RemoveVisibility 
        {
            get
            {
                return _isNew ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public ICommand GoBack { get; private set; }

        public LambdaCommand Ok { get; private set; }

        public ICommand Remove { get; private set; }

        public KnownPersonEditorViewModel() : this("иван", "иванов", true) { }

        public KnownPersonEditorViewModel(string firstName, string surname, bool isBoy = true)
            : this(Essentials.CreateKnownPerson(firstName, surname, isBoy)) 
        {
            _isBoy = isBoy;
        }

        public KnownPersonEditorViewModel(KnownPerson person)
        {
            if (person == null)
                throw new ArgumentNullException("person");

            _person = person;

            GoBack = new LambdaCommand()
            {
                ExecuteAction = MainWindow.Current.CloseModal
            };

            Ok = new LambdaCommand()
            {
                CanExecuteFunc = ValidateForm,
                ExecuteAction = Save
            };

            Remove = new LambdaCommand()
            {
                ExecuteAction = RemoveImpl
            };

            _isNew = person.Id == 0;

            Fill();
        }

        string ToTitleCase(string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.Trim().ToLower());
        }

        void SetNoticeVisibility()
        {
            var knownName = KnownNamesSearcher.Search(_person.NominativeName);
            if (knownName == null || knownName.Case != "Nominative")
            {
                IsBadNameVisibility = Visibility.Visible;
                IsAutoInflectedVisibility = Visibility.Collapsed;

                return;
            }
            else
            {
                IsBadNameVisibility = Visibility.Collapsed;
                IsAutoInflectedVisibility = _isNew ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void Fill()
        {
            NominativeName = ToTitleCase(_person.NominativeName);
            NominativeSurname = ToTitleCase(_person.NominativeSurname);
            GenitiveName = ToTitleCase(_person.GenitiveName);
            GenitiveSurname = ToTitleCase(_person.GenitiveSurname);
            DativeName = ToTitleCase(_person.DativeName);
            DativeSurname = ToTitleCase(_person.DativeSurname);
            AccusativeName = ToTitleCase(_person.AccusativeName);
            AccusativeSurname = ToTitleCase(_person.AccusativeSurname);
            InstrumentalName = ToTitleCase(_person.InstrumentalName);
            InstrumentalSurname = ToTitleCase(_person.InstrumentalSurname);
            PrepositionalName = ToTitleCase(_person.PrepositionalName);
            PrepositionalSurname = ToTitleCase(_person.PrepositionalSurname);

            NominativeFullName = new Person()
            {
                FirstName = NominativeName,
                LastName = NominativeSurname
            }.FullName.ToUpper();

            SetNoticeVisibility();
        }

        void Save()
        {
            _person.NominativeName = NominativeName.Trim().ToLower();
            _person.NominativeSurname = NominativeSurname.Trim().ToLower();
            _person.GenitiveName = GenitiveName.Trim().ToLower();
            _person.GenitiveSurname = GenitiveSurname.Trim().ToLower();
            _person.DativeName = DativeName.Trim().ToLower();
            _person.DativeSurname = DativeSurname.Trim().ToLower();
            _person.AccusativeName = AccusativeName.Trim().ToLower();
            _person.AccusativeSurname = AccusativeSurname.Trim().ToLower();
            _person.InstrumentalName = InstrumentalName.Trim().ToLower();
            _person.InstrumentalSurname = InstrumentalSurname.Trim().ToLower();
            _person.PrepositionalName = PrepositionalName.Trim().ToLower();
            _person.PrepositionalSurname = PrepositionalSurname.Trim().ToLower();

            if (_isNew)
                Context.Default.KnownPersons.Add(_person);

            Tools.MergeKnownNameWithKnownPerson(_person);

            Context.Default.SaveChanges();

            GoBack.Execute(null);
        }

        bool ValidateForm()
        {
            return
                ValidateString(NominativeName) &&
                ValidateString(NominativeSurname) &&
                ValidateString(GenitiveName) &&
                ValidateString(GenitiveSurname) &&
                ValidateString(DativeName) &&
                ValidateString(DativeSurname) &&
                ValidateString(AccusativeName) &&
                ValidateString(AccusativeSurname) &&
                ValidateString(InstrumentalName) &&
                ValidateString(InstrumentalSurname) &&
                ValidateString(PrepositionalName) &&
                ValidateString(PrepositionalSurname);
        }

        bool ValidateString(string text)
        {
            return text != null && text.Trim().Length > 0;
        }

        void Discard()
        {
        }

        void RemoveImpl()
        {
            if (_isNew)
                return;

            Context.Default.KnownPersons.Remove(_person);
            Context.Default.SaveChanges();

            GoBack.Execute(new { });
        }
    }
}
