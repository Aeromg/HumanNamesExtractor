namespace IndexerLib.Persist
{
    public class KnownName : EntityBase<KnownName>
    {
        public string Nominative { get; set; }
        public string Genitive { get; set; }
        public string Dative { get; set; }
        public string Accusative { get; set; }
        public string Instrumental { get; set; }
        public string Prepositional { get; set; }
        public bool IsMale { get; set; }
    }
}
