using System.Collections.Generic;

namespace Semanticer.Classifier.Transformers
{
    public interface ITextTransformer
    {
        IEnumerable<SparseNumericFeature> Transform(string[] words);
        IEnumerable<SparseNumericFeature> Transform(string sentence);
    }
}