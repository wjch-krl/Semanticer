using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Numeric
{
    public class ClassifiedSentence : ClassifiableSentence
    {
        public MarkType Label { get; set; }
        public string[] Words { get; set; }
    }
}