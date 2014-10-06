using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace IndexerLib.Persist
{
    public class Context : DbContext
    {
        private static Context _default;
        private static ContextCached _cached;

        public DbSet<KnownName> KnownNames { get; set; }
        public DbSet<KnownPerson> KnownPersons { get; set; }
        public DbSet<StopWord> StopWords { get; set; }

        public static Context Default { get { return _default ?? (_default = new Context()); } }

        public static ContextCached Cached { get { return _cached ?? (_cached = new ContextCached()); } }

        private Context() : base("db")
        {
            ((IObjectContextAdapter)this).ObjectContext.SavingChanges += Context_SavingChanges;
        }

        static Context()
        {
            // Database.SetInitializer<Context>(null);
        }

        static void Context_SavingChanges(object sender, object e)
        {
            var context = sender as ObjectContext;
            if (context == null) return;

            foreach (var entry in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified))
                (entry.Entity as EntityBase).BeforeSave();

            if (_cached != null)
                _cached.Reset();
        }
    }

    public class ContextCached
    {
        public event EventHandler WarmingBegins;
        public event EventHandler WarmingEnds;

        private IDictionary<string, KnownName> _knownNamesHashSet;

        private IEnumerable<KnownName> _knownNames;
        private IEnumerable<KnownPerson> _knownPerson;
        private IEnumerable<StopWord> _stopWords;

        public IDictionary<string, KnownName> KnownNamesHash
        {
            get { return _knownNamesHashSet ?? (_knownNamesHashSet = GetKnownNamesHashSet()); }
        }

        public IEnumerable<KnownName> KnownNames
        {
            get { return _knownNames ?? (_knownNames = Context.Default.KnownNames.ToArray()); }
        }

        public IEnumerable<KnownPerson> KnownPersons
        {
            get { return _knownPerson ?? (_knownPerson = Context.Default.KnownPersons.ToArray()); }
        }

        public IEnumerable<StopWord> StopWords
        {
            get { return _stopWords ?? (_stopWords = Context.Default.StopWords.ToArray()); }
        }

        public void Reset()
        {
            lock (this)
            {
                _knownNamesHashSet = null;
                _knownNames = null;
                _knownPerson = null;
                _stopWords = null;
            }
        }

        public void Warm()
        {
            OnWarmingBegins();

            Reset();
            GC.Collect();

            lock (this)
            {
                _knownNames = Context.Default.KnownNames.ToArray();
                _knownPerson = Context.Default.KnownPersons.ToArray();
                _stopWords = Context.Default.StopWords.ToArray();
                _knownNamesHashSet = GetKnownNamesHashSet();
            }

            OnWarmingEnds();
        }

        private IDictionary<string, KnownName> GetKnownNamesHashSet()
        {
            var set = new Dictionary<string, KnownName>();
            foreach (var knownName in KnownNames)
            {
                set[knownName.Nominative] = knownName;
                set[knownName.Genitive] = knownName;
                set[knownName.Dative] = knownName;
                set[knownName.Accusative] = knownName;
                set[knownName.Instrumental] = knownName;
                set[knownName.Prepositional] = knownName;
            }

            return set;
        }

        private void OnWarmingBegins()
        {
            var handler = WarmingBegins;

            if(handler ==null)
                return;

            handler(this, EventArgs.Empty);
        }

        private void OnWarmingEnds()
        {
            var handler = WarmingEnds;

            if (handler == null)
                return;

            handler(this, EventArgs.Empty);
        }
    }
}
