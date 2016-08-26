using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Numeric.kNN
{
    public class ClassifiedSentence : ClassifiableSentence
    {
        public PostMarkType Label { get; set; }
        public string[] Words { get; set; }
    }
}