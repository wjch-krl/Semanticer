using Semanticer.Classifier;

namespace Semanticer.TextAnalyzer
{
    public interface ITokenizerFactory
    {
        ITokenizer Create();
    }
}