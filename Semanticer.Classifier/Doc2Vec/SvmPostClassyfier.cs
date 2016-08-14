using System;
using System.Collections.Generic;
using System.Linq;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Doc2Vec
{
    public class SvmPostClassyfier : IPostClassyfier
    {
        private IPostTransformer transformer;

        public SvmPostClassyfier(IPostTransformer transformer)
        {
            this.transformer = transformer;
        }

        public IEnumerable<Post> ClassyfyPost(ICollection<Post> posts, ICollection<Post> trainData)
        {
            var transformed = transformer.Transform(posts,trainData);
            var classified = Classify(transformed.TrainSentences, transformed.SentencesToClassify);
            return GetPost(posts, classified);
        }

        public IEnumerable<Post> GetPost(IEnumerable<Post> posts, IEnumerable<ClassifiedSentence> sentences)
        {
            var sentencesLookup = sentences.ToLookup(x => x.FullId, x => x);
            foreach (var post in posts)
            {
                if (!sentencesLookup.Contains(post.FullId))
                {
                    yield return post;
                }
                AssignPostMarkFromSentences(sentencesLookup, post);
                yield return post;
            }
        }

        private static void AssignPostMarkFromSentences(ILookup<string, ClassifiedSentence> classifiedPosts, Post post)
        {
            var sentences = classifiedPosts[post.FullId];
            var marks = sentences.Select(x => x.MarkType).WeightedAvg();
            post.MarkType = marks.ToPostMarkType();
            post.Mark = marks;
            post.MarkValue = post.Mark*post.Strong;
        }

        private IEnumerable<ClassifiedSentence> Classify(IEnumerable<ClassifiedSentence> trainSentences, ICollection<ClassifiableSentence> toClassyfy)
        {
            var parameter = new SVMParameter
            {
                Kernel = SVMKernelType.RBF,
                C = 0.1767767,
                Gamma = 0.3535534,
            };
            SVMProblem trainProblem = SvmProblem(trainSentences.Select(x => Tuple.Create(x.MarkType,x.Features)));
            ScaleNewProblem(trainProblem);
            var classifier = trainProblem.Train(parameter);
            trainProblem.Save("C:\\train.data.svm");
            var toPredict = SvmProblem(toClassyfy.Select(x => x.Features));
            toPredict.Predict(classifier);
            return toClassyfy.Select((elm, idx) => new ClassifiedSentence(elm, toPredict.Y[idx].ToPostMarkType()));
        }

        private SVMProblem SvmProblem(IEnumerable<Tuple<PostMarkType, double[]>> porob)
        {
            var problem = new SVMProblem();
            foreach (var element in porob)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                problem.Add(element.Item2.Where(x => x != 0.0).Select((t, i) => new SVMNode(i + 1, t)).ToArray(), 2 - (int)element.Item1);
            }
            return problem;
        }

        private static SVMProblem SvmProblem(IEnumerable<IEnumerable<double>> porob)
        {
            var problem = new SVMProblem();
            foreach (var element in porob)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                problem.Add(element.Where(x => x != 0.0).Select((t, i) => new SVMNode(i + 1, t)).ToArray(), 0);
            }
            return problem;
        }

        private void ScaleNewProblem(SVMProblem problem)
        {
            var scale = new double[problem.X.SelectMany(x => x).Max(x => x.Index), 2];
            foreach (var featureGroups in problem.X.SelectMany(x => x).GroupBy(x => x.Index))
            {
                var featureValue = featureGroups.Select(x => x.Value).ToArray();
                scale[featureGroups.Key - 1, 0] = featureValue.Average();
                scale[featureGroups.Key - 1, 1] = featureValue.CalculateStdDev();
            }
            ScaleProblem(problem,scale);
        }

        private void ScaleProblem(SVMProblem problem, double[,] scale)
        {
            for (int i = 0; i < problem.X.Count; i++)
            {
                problem.X[i] = problem.X[i].Select(x =>
                {
                    x.Value = (x.Value - scale[x.Index - 1, 0]) / scale[x.Index - 1, 1];
                    return x;
                }).ToArray();
            }
        }
    }
}