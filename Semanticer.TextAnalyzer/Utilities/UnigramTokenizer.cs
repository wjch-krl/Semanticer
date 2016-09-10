using System;
using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer.Utilities
{
    public class UnigramTokenizer : ITokenizer
    {
        public string[] Tokenize(string input)
        {
            var splitted = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return splitted;
        }
    }
}