using System;
using System.Collections.Generic;
using System.Linq;

namespace IndexerLib.RegexExtension
{
    public static class RegexFunctionResolver
    {
        static string[] _functionNames;

        static IDictionary<string, Type> Functions { get; set; }

        public static IEnumerable<string> FunctionNames
        {
            get
            {
                return _functionNames ?? (_functionNames = Functions.Keys.ToArray());
            }
        }

        public static IRegexFunction Resolve(string name)
        {
            return 
                Activator.CreateInstance(Functions[name]) as IRegexFunction;
        }

        public static void RegisterFunction(Type type)
        {
            var name = GetFunctionName(type);
            Functions[name] = type;

            _functionNames = null;
        }

        public static string GetFunctionName(IRegexFunction function)
        {
            return GetFunctionName(function.GetType());
        }

        static RegexFunctionResolver()
        {
            Functions = new Dictionary<string, Type>();
        }

        static string GetFunctionName(Type type)
        {
            var attribute =
                type.GetCustomAttributes(typeof(RegexFunctionInfoAttribute), true).
                Cast<RegexFunctionInfoAttribute>().
                FirstOrDefault();

            return attribute.Name;
        }
    }
}
