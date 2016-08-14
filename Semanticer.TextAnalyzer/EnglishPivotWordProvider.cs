using System.Collections.Generic;
using Semanticer.Classifier;

namespace Semanticer.TextAnalyzer
{
    public class EnglishPivotWordProvider : IPivotWordProvider
    {
        private readonly Dictionary<string, double> pivotWords;

        public EnglishPivotWordProvider()
        {
            pivotWords = new Dictionary<string, double> {{"not", -1.0}, {"very", 5.0}, {"none", -1}};
        }

        public bool IsPivot(string word)
        {
            return pivotWords.ContainsKey(word);
        }

        public double Multiper(string word)
        {
            return pivotWords.ContainsKey(word) ? pivotWords[word] : 1.0;
        }
    }
}