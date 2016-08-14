using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Doc2Vec
{
    public class ClassifiedSentence : ClassifiableSentence
    {
        public ClassifiedSentence(ClassifiableSentence classifiableSentence, PostMarkType markType)
            : base(classifiableSentence.Features, classifiableSentence.SentenceNumber, classifiableSentence.OrgId, classifiableSentence.SourceId)
        {
            MarkType = markType;
        }

        public PostMarkType MarkType { get; set; }
    }
}