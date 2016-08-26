using Semanticer.Classifier.Transformers;

namespace Semanticer.Classifier.Numeric.kNN
{
    public class ClassifiableSentence
    {
        public long SentenceNumber { get; set; }
        public SparseNumericFeature[] Features { get; set; }
    }
}