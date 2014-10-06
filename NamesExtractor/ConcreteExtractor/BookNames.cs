using System;

namespace IndexerLib.ConcreteExtractorExtractor
{
    class BookNames : IPersonReference
    {
        public IPage Page
        {
            get { throw new NotImplementedException(); }
        }

        public NameProbability Probability
        {
            get { throw new NotImplementedException(); }
        }

        public Person Person
        {
            get { throw new NotImplementedException(); }
        }

        public Token Token
        {
            get { throw new NotImplementedException(); }
        }
    }
}
