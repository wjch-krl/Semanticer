using Semanticer.Classifier.Transformers;

namespace Semanticer.TextAnalyzer
{
    public class BagOfWordsTransformerFactory : TextTransformerFactory
    {
        public override ITextTransformer CreateTextTransformer()
        {
            return new BagOfWordsTransformer(TokenizerFactory.Create(),PivotFactory.Resolve(Lang));
        }
    }
}