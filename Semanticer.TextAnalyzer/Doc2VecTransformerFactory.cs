using Semanticer.Classifier.Transformers;
using Semanticer.Classifier.Transformers.Doc2Vec;

namespace Semanticer.TextAnalyzer
{
    public class Doc2VecTransformerFactory : TextTransformerFactory
    {
        private readonly Doc2VecTransformer transformer;

        public Doc2VecTransformerFactory()
        {
            this.transformer = new Doc2VecTransformer(new Doc2VecArgs());
        }

        public override ITextTransformer CreateTextTransformer()
        {
            return transformer;
        }
    }
}