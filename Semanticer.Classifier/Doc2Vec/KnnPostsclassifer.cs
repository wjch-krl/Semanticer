using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Semanticer.Common.DataModel;

namespace Semanticer.Classifier.Doc2Vec
{
    public class KnnPostsClassifer : IPostClassyfier
    {
        private readonly IPostTransformer transformer;

        public KnnPostsClassifer(IPostTransformer transformer)
        {
            this.transformer = transformer;
        }

        public IEnumerable<Post> ClassyfyPost(ICollection<Post> posts, ICollection<Post> trainData)
        {
            var trainDict = trainData.ToDictionary(x => x.FullId, x => x);
            var testDict = posts.ToDictionary(x => x.FullId, x => x);
            var transformed = transformer.Transform(posts, trainData);
            var knn = new KNearestNeighbors<ClassifiableSentence>(3,
                transformed.TrainSentences.Cast<ClassifiableSentence>().ToArray(),
                transformed.TrainSentences.Select(x => 2 - (int) x.MarkType).ToArray(), (x, y) =>
                {
                    double sum = x.Features.Select((t, i) => Math.Pow(t - y.Features[i], 2.0)).Sum();
                    return Math.Sqrt(sum);
                });

            foreach (var post in transformed.SentencesToClassify)
            {
                int[] labels;
                var neightbours = knn.GetNearestNeighbors(post, out labels);
                var testMsg = testDict[post.FullId];
                var neightbourPosts = neightbours.Select(x => trainDict[x.FullId]);
                var mark = neightbourPosts.Select(x => x.MarkType).WeightedAvg1();
                testMsg.MarkType = mark;
                testMsg.Mark = 2 - (int) mark;
                testMsg.MarkValue = testMsg.Mark*testMsg.Strong;
                yield return testMsg;
            }
        }
    }
}