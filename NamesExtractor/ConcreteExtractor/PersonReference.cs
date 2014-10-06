namespace IndexerLib.ConcreteExtractor
{
    class PersonReference : IPersonReference
    {
        public NameProbability Probability { get; set; }

        public Person Person { get; set; }

        public Token Token { get; set; }

        public IPage Page { get; set; }
    }
}
