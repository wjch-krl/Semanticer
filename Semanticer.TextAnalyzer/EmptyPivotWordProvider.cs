using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{
    public class EmptyPivotWordProvider : IPivotWordProvider
    {
        public bool IsPivot(string word)
        {
            return false;
        }

        public double Multiper(string word)
        {
            return 1.0;
        }
    }
}