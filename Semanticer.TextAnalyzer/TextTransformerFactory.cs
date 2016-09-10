using Semanticer.Classifier.Common;
using Semanticer.Classifier.Transformers;

namespace Semanticer.TextAnalyzer
{
    public abstract class TextTransformerFactory
    {
        public abstract ITextTransformer CreateTextTransformer();
        public string Lang { protected get; set; }

        public IPivotWordProviderFactory PivotFactory { protected get; set; }

        public ITokenizerFactory TokenizerFactory { protected get; set; }

    }
}