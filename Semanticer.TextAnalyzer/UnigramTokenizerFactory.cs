using Semanticer.Classifier.Common;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer
{
    public class UnigramTokenizerFactory:ITokenizerFactory
    {
        public ITokenizer Create()
        {
            return new UnigramPostTokenizer();
        }
    }
}