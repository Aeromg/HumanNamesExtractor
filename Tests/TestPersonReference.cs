using IndexerLib;

namespace Tests
{
    class TestPersonReference : IPersonReference
    {
        public IPage Page { get; set; }

        public NameProbability Probability { get; set; }

        public Person Person { get; set; }

        public Token Token { get; set; }
    }
}
