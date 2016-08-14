using System;
using Semanticer.Classifier;

namespace Semanticer.TextAnalyzer.Utilities
{
    public class UnigramPostTokenizer : ITokenizer
    {
        public string[] Tokenize(string input)
        {
            var splitted = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return splitted;
        }
    }
}