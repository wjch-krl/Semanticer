using Semanticer.Classifier.Transformers;

namespace Semanticer.Classifier.Numeric
{
    public class ClassifiableSentence
    {
        public long SentenceNumber { get; set; }
        public SparseNumericFeature[] Features { get; set; }
    }
}