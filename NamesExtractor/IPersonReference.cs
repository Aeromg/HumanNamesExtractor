namespace IndexerLib
{
    public interface IPersonReference
    {
        IPage Page { get; }
        NameProbability Probability { get; }
        Person Person { get; }
        Token Token { get; }
    }
}
