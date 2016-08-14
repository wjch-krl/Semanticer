using System.Collections.Generic;

namespace Semanticer.Classifier
{
    public class PolishPivotWordProvider : IPivotWordProvider
    {
        private readonly Dictionary<string, double> pivotWords;

        public PolishPivotWordProvider()
        {
            pivotWords = new Dictionary<string, double> {{"nie", -1.0}, {"bardzo", 5.0}, {"niezbyt",-0.1}};
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
