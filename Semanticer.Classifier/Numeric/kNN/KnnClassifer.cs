using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Transformers;
using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Numeric.kNN
{
    public class KnnClassifer : NumericClassiferBase, ITrainable
    {
        private KNearestNeighbors<ClassifiableSentence> knn;
        private const int NeighborCount = 5;
        private const double MaxDistance = 1.0;

        public KnnClassifer(ITextTransformer transformer) : base(transformer)
        {
        }

        public long FeatureCount { get; private set; }

        public string GetNearestNeighbors(string input)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            var post = SentenceFromString(input);
            int[] labels;
            var neighbours = knn.GetNearestNeighbors(post, out labels);
            return TransformPrediction(labels);
        }

        private static Dictionary<PostMarkType, double> TransformPrediction(int[] labels)
        {
            return labels.Select(x => (PostMarkType) x)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, y => (double) y.Count()/NeighborCount);
        }

        public TimeSpan ReTrain(ITrainingData data)
        {
            var dateStart = DateTime.UtcNow;
            var trainEvents = ExtranctEnumerableTrainEvents(data).ToArray();
            this.FeatureCount = trainEvents.LongLength;
            var words = GetWords(trainEvents);
            Transformer.AddAllWords(words);
            var trainSet = ProccesTrainingData(trainEvents);
            var outcomes = trainSet.Select(x => (int) x.Label).ToArray();
            var toTrain = trainSet.Cast<ClassifiableSentence>().ToArray();
            knn = new KNearestNeighbors<ClassifiableSentence>(NeighborCount, toTrain, outcomes, Distance);
            return DateTime.UtcNow - dateStart;
        }


        private double Distance(ClassifiableSentence x, ClassifiableSentence y)
        {
            int commonFeaturesCount = x.Features.Join(y.Features, xVal => xVal.FeatureId, yVal => yVal.FeatureId, (a, b) => a).Count();
            //double sum = commonFeaturesDist.Sum();
            double sum = FeatureCount - commonFeaturesCount;
            return Math.Sqrt(sum);
        }

        private static double AlienFeatureDistance(ClassifiableSentence x, ClassifiableSentence y, int commonFeaturesCount)
        {
            return (x.Features.Length + y.Features.Length - 2*commonFeaturesCount)*MaxDistance;
        }

        private double TwoFeaturesDistance(SparseNumericFeature f1, SparseNumericFeature f2)
        {
            return Math.Pow(f1.Value - f2.Value, 2.0);
        }
    }
}
