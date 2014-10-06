using System.Linq;
using System.Text.RegularExpressions;

namespace IndexerLib.RegexExtension
{
    public static class RegexFunctionExecutor
    {
        static readonly Regex QuoteRegex = new Regex(@"(?<!\\)\$");

        public static string ExecuteExpression(string expression)
        {
            var extracted = expression;

            while (extracted != "")
            {
                extracted = ExtractDeepestExpression(expression);
                if (extracted == "")
                    break;

                expression = expression.Replace(extracted, ExecuteSingleExpression(extracted));
            }

            return expression;
        }

        static string ExecuteSingleExpression(string expression)
        {
            var funcName = expression.Substring(0, expression.IndexOf(@"$"));
            var args = expression.Substring(funcName.Length + 1, expression.Length - funcName.Length - 2);

            var func = RegexFunctionResolver.Resolve(funcName);

            return func.Execute(args);
        }

        static string ExtractDeepestExpression(string expression)
        {
            var quotes = QuoteRegex.Matches(expression).
                Cast<Match>().
                Select(m => m.Index).
                Reverse().
                ToArray();

            if (quotes.Length % 2 != 0)
                return "";

            var names = RegexFunctionResolver.FunctionNames.OrderBy(f => f.Length).ToArray();

            for (var qId = 1; qId < quotes.Length; qId++)
            {
                for (int fId = 0; fId < names.Length; fId++)
                {
                    if (names[fId].Length > quotes[qId])
                        break;

                    var nameStart = quotes[qId] - names[fId].Length;
                    var substr = expression.Substring(nameStart, names[fId].Length);
                    if (substr == names[fId])
                    {
                        var function = substr;
                        var args = expression.Substring(quotes[qId], quotes[qId - 1] - quotes[qId] + 1);

                        return function + args;
                    }
                }
            }

            return "";
        }

        static RegexFunctionExecutor() 
        {
            RegexFunctionResolver.RegisterFunction(typeof(InflectRegexFunction));
            RegexFunctionResolver.RegisterFunction(typeof(WordRegexFunction));
        }
    }
}
