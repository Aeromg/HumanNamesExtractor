using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IndexerLib.Lingva;
using IndexerLib.Persist;
using NPetrovich;

namespace IndexerLib
{
    public class Person
    {
        private static readonly IPersonNameParser _defaultParser = new AutoOrderPersonNameParser();

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(String.Format(@"{0} {1}", FirstName, LastName));
            }
        }

        public static Person Parse(string fullName, IPersonNameParser parser = null)
        {
            return (parser ?? _defaultParser).Parse(fullName);
        }

        public override string ToString()
        {
            return FullName;
        }

        public static bool TryParse(string SearchText, out Person person)
        {
            try
            {
                person = Parse(SearchText);
                return true;
            }
            catch
            {
                person = null;
                return false;
            }
        }
    }

    class AutoOrderPersonNameParser : IPersonNameParser
    {
        static readonly string[] MiddleNameEndings = { "ович", "евич", "овна", "евна", "ична" };
        private static readonly string[] LastNameEndings = { "ев", "ин", "ов", "ева", "ина", "ова" };

        const int MinimumNameTokens = 2;
        const int MinimumNameTokenLength = 2;

        public Person Parse(string fullName)
        {
            if (String.IsNullOrEmpty(fullName))
                throw new ArgumentNullException("fullName");

            string[] names = GetNames(ref fullName);
            int lastNameIndex = -1;
            int firstNameIndex = GetNominativeFirstNameIndex(ref names);
            if (firstNameIndex != -1)
            {
                lastNameIndex = GetNominativeKnownLastNameIndex(ref names);
                if (lastNameIndex == -1 || lastNameIndex == firstNameIndex)
                {
                    lastNameIndex = GetNominativeLastNameIndex(ref names);
                    if (lastNameIndex == -1 || lastNameIndex == firstNameIndex)
                        lastNameIndex = GetDefaultLastNameIndex(ref names, firstNameIndex);
                }

                return BuildPerson(ref names, firstNameIndex, lastNameIndex);
            }
            
            firstNameIndex = GetFirstNameIndex(ref names);
            lastNameIndex = GetNominativeLastNameIndex(ref names);

            if (firstNameIndex == -1 && lastNameIndex != -1)
                firstNameIndex = GetDefaultFirstNameIndex(ref names, lastNameIndex);

            if (lastNameIndex == -1 || lastNameIndex == firstNameIndex)
                lastNameIndex = GetDefaultLastNameIndex(ref names, firstNameIndex);

            if (lastNameIndex == -1)
                lastNameIndex = GetDefaultLastNameIndex(ref names, firstNameIndex);

            return firstNameIndex == -1 ? 
                GetUnknownPerson(ref names) :
                BuildPerson(ref names, firstNameIndex, lastNameIndex);
        }

        static Person BuildPerson(ref string[] names, int firstNameIndex, int lastNameIndex)
        {
            return new Person()
            {
                FirstName = names[firstNameIndex],
                LastName = names[lastNameIndex]
            };
        }

        static Person GetUnknownPerson(ref string[] names)
        {
            return new Person()
            {
                FirstName = names[0],
                LastName = names[1]
            };
        }

        static int GetFirstNameIndex(ref string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                var result = KnownNamesSearcher.Search(names[i]);

                if (result != null)
                    return i;
            }

            return -1;
        }

        static int GetNominativeFirstNameIndex(ref string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                var result = KnownNamesSearcher.Search(names[i]);
                
                if (result == null)
                    continue;

                if (result.Case == @"Nominative")
                    return i;
            }

            return -1;
        }

        static int GetNominativeKnownLastNameIndex(ref string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                if (Context.Cached.KnownPersons.Any(p => p.NominativeSurname == name))
                    return i;
            }

            return -1;
        }

        static int GetNominativeLastNameIndex(ref string[] names)
        {
            for(int i = 0; i < names.Length; i++)
                if (CheckIfLastNameEndings(ref names[i]))
                    return i;

            return -1;
        }

        static int GetDefaultLastNameIndex(ref string[] names, int firstNameIndex)
        {
            if (names.Length == 2)
                return firstNameIndex == 0 ? 1 : 0;

            if (firstNameIndex == 0 && CheckIfMiddleNameEnding(ref names[1]))
                return 2;

            if (firstNameIndex == 1 && CheckIfMiddleNameEnding(ref names[2]))
                return 0;

            return
                firstNameIndex == names.Length - 1 ? firstNameIndex - 1 : firstNameIndex + 1;
        }

        static int GetDefaultFirstNameIndex(ref string[] names, int lastNameIndex)
        {
            if (names.Length == 2)
                return lastNameIndex == 0 ? 1 : 0;

            return
                lastNameIndex == names.Length - 1 ? 0 : names.Length - 1;
        }

        static bool CheckIfMiddleNameEnding(ref string name)
        {
            for (int j = 0; j < MiddleNameEndings.Length; j++)
                if (name.EndsWith(MiddleNameEndings[j]))
                    return true;

            return false;
        }

        static bool CheckIfLastNameEndings(ref string name)
        {
            for (int j = 0; j < LastNameEndings.Length; j++)
                if (name.EndsWith(LastNameEndings[j]))
                    return true;

            return false;
        }

        static string[] GetNames(ref string fullName)
        {
            var names = fullName.Split(' ');

            int validNamesCount = 0;
            var validNames = new string[names.Length];

            for (var i = 0; i < names.Length; i++)
            {
                var validName = names[i].Trim().ToLower();
                if (validName.Length >= MinimumNameTokenLength)
                {
                    validNames[validNamesCount] = validName;
                    validNamesCount++;
                }
            }

            if(validNamesCount < MinimumNameTokens)
                throw new Exception("Parsing error");

            var result = new string[validNamesCount];
            Array.Copy(validNames, result, validNamesCount);

            return result;
        }
    }
}
