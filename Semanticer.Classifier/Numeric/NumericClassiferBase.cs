using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Transformers;
using SharpEntropy;

namespace Semanticer.Classifier.Numeric
{
    public abstract class NumericClassiferBase : ClassifierBase
    {
        private static long _sentenceIdSeed;
        protected readonly ITextTransformer Transformer;

        protected NumericClassiferBase(ITextTransformer transformer)
        {
            this.Transformer = transformer;
        }

        protected ClassifiableSentence SentenceFromString(string input)
        {
            var features = Transformer.Transform(input);
            long sentenceId = SentenceId();
            return new ClassifiableSentence
            {
                Features = features.ToArray(),
                SentenceNumber = sentenceId,
            };
        }

        private static long SentenceId()
        {
            return Interlocked.Increment(ref _sentenceIdSeed);
        }

        protected IEnumerable<string> GetWords(TrainingEvent[] trainEvents)
        {
            return trainEvents.SelectMany(x => x.GetContext());
        }

        protected IEnumerable<TrainingEvent> ExtranctEnumerableTrainEvents(ITrainingEventReader reader)
        {
            while (reader.HasNext())
            {
                var sentene = reader.ReadNextEvent();
                yield return sentene;
            }
        }

        protected IEnumerable<TrainingEvent> ExtranctEnumerableTrainEvents(ITrainingData data)
        {
            var reader = data.Reader;
            return ExtranctEnumerableTrainEvents(reader);
        }

        protected ClassifiedSentence[] ProccesTrainingData(IEnumerable<TrainingEvent> items)
        {
            return items.Select(TrainedSentenceFromTrainEvent).ToArray();
        }

        private ClassifiedSentence TrainedSentenceFromTrainEvent(TrainingEvent trainingEvent)
        {
            var features = Transformer.Transform(trainingEvent.GetContext());
            long sentenceId = SentenceId();
            var mark = trainingEvent.GetMarkType();
            return new ClassifiedSentence
            {
                Features = features.ToArray(),
                Label = mark,
                SentenceNumber = sentenceId,
                Words = trainingEvent.GetContext()
            };
        }
    }
}