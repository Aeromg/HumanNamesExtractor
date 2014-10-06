using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace IndexerLib.Persist
{
    public class KnownPerson : EntityBase<KnownPerson>
    {
        public string NominativeName { get; set; }
        public string NominativeSurname { get; set; }

        [NotMapped]
        public string NominativeFullName
        {
            get
            {
                return new Person()
                {
                    FirstName = NominativeName,
                    LastName = NominativeSurname
                }.FullName;
            }
        }

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

        public string IndexedName { get; set; }

        public override void BeforeSave()
        {
            IndexedName = NominativeFullName.ToLower();
            base.BeforeSave();
        }

        internal static IEnumerable<KnownPerson> SearchByText(string text)
        {
            text = text.Trim().ToLower();

            return
                from p in Context.Default.KnownPersons
                where p.IndexedName.Contains(text)
                select p;
        }
    }
}
