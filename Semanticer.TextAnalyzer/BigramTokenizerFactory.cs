using Semanticer.Classifier;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer
{
    public class BigramTokenizerFactory : ITokenizerFactory
    {
        public ITokenizer Create()
        {
            return new BigramPostTokenizer();
        }
    }
}