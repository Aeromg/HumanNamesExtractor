using NPetrovich;

namespace IndexerLib.Lingva
{
    public class MyOwnPetrovich
    {
        private readonly Petrovich _petrovich = new Petrovich();
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private Gender _gender;
        private bool _autoGender = true;

        public string FirstName
        {
            get { return _petrovich.FirstName; }
        }

        public string MiddleName
        {
            get { return _petrovich.MiddleName; }
        }

        public string LastName
        {
            get { return _petrovich.LastName; }
        }

        public Gender Gender
        {
            get { return _autoGender ? _petrovich.Gender : _gender; }
        }

        public void SetNominative(string firstName = "", string middleName = "", string lastName = "")
        {
            _firstName = firstName;
            _middleName = middleName;
            _lastName = lastName;
        }

        public void SetGender(Gender gender)
        {
            _autoGender = false;
            _gender = gender;
            Reset();
        }

        public void InflectTo(Case @case)
        {
            Reset();
            _petrovich.InflectTo(@case);
        }

        public string InflectFirstNameTo(Case @case)
        {
            Reset();
            return _petrovich.InflectFirstNameTo(@case);
        }

        public string InflectLastNameTo(Case @case)
        {
            Reset();
            return _petrovich.InflectLastNameTo(@case);
        }

        public string InflectMiddleNameTo(Case @case)
        {
            Reset();
            return _petrovich.InflectMiddleNameTo(@case);
        }

        void Reset()
        {
            _petrovich.FirstName = _firstName;
            _petrovich.MiddleName = _middleName;
            _petrovich.LastName = _lastName;
            //_petrovich.AutoDetectGender = _autoGender;
            
            _petrovich.Gender = Gender;
        }
    }
}
