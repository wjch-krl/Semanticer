using System.Collections.Generic;

namespace Semanticer.Classifier.Doc2Vec
{
    public class ClassificatorData
    {
        public ClassificatorData(ICollection<ClassifiedSentence> trainSentences,
            ICollection<ClassifiableSentence> sentencesToClassify)
        {
            TrainSentences = trainSentences;
            SentencesToClassify = sentencesToClassify;
        }

        public ICollection<ClassifiedSentence> TrainSentences { get; private set; }
        public ICollection<ClassifiableSentence> SentencesToClassify { get; private set; }
    }
}