using System;
using System.Collections.Generic;
using System.Text;
using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer.Utilities
{
    public class NgramPostTokenizer : ITokenizer
    {
        private readonly int ngramCount;

        public NgramPostTokenizer(int ngramCount = 2)
        {
            this.ngramCount = ngramCount;
        }

        public virtual string[] Tokenize(string input)
        {
            var splitted = input.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            return CreateNgram(splitted);
        }

        protected string[] CreateNgram(string[] splitted)
        {
            if (splitted.Length == 0)
            {
                return splitted;
            }
            return Ngrams(splitted);
        }

        private string[] Ngrams(string[] words)
        {
            List<string> ngrams = new List<string>();
            for (int i = 0; i < words.Length - ngramCount + 1; i++)
            {
                ngrams.Add(Concat(words, i, i + ngramCount));
            }
            return ngrams.ToArray();
        }

        private string Concat(string[] words, int start, int end)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < end; i++)
            {
                sb.Append((i > start ? " " : "") + words[i]);
            }
            return sb.ToString();
        }
    }
}