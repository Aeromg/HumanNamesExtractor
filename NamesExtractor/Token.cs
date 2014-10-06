namespace IndexerLib
{
    public class Token
    {
        public string Lookup { get; set; }

        public string Text { get; set; }

        private string _lowerText = null;

        public string LowerText 
        {
            get
            {
                return _lowerText ?? (_lowerText = Text.Trim().ToLower());
            }
        }

        public int Index { get; internal set; }
    }
}
