using System;

namespace IndexerLib.RegexExtension
{
    class RegexFunctionInfoAttribute : Attribute
    {
        public string Name { get; set; }

        public string ArgumentPattern { get; set; }

        public RegexFunctionInfoAttribute(string name) 
            : this(name, "") { }

        public RegexFunctionInfoAttribute(string name, string argumentPattern)
            : base()
        {
            Name = name;
            ArgumentPattern =argumentPattern;
        }
    }
}
