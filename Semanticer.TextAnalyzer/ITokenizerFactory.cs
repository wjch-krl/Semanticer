using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{
    public interface ITokenizerFactory
    {
        ITokenizer Create();
    }
}