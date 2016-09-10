using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Transformers;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;
using SharpEntropy;

namespace Semanticer.Classifier.Numeric.Svm
{
    public class Svm : NumericClassiferBase, ITrainable, ISerializableClassifier
    {
        private double[,] Scale;
        private SVMModel svMachine;

        public Svm(ITextTransformer transformer) : base(transformer)
        {
        }

        public override IDictionary<MarkType, double> Classify(string input)
        {
            var porob = Transformer.Transform(input);
            var problem = SvmProblem(porob);
            ScaleProblem(problem);
            var x = problem.X[0];
            var prediction = svMachine.Predict(x);
            return new Dictionary<MarkType, double> {{(MarkType) (int) prediction, 1}};
        }

        public TimeSpan ReTrain(ITrainingData data)
        {
            var parameter = CreateSvmParameter();
            var starTime = DateTime.Now;
            var events = ExtranctEnumerableTrainEvents(data);
            var problem = CreateTrainProblem(events);
            ScaleNewProblem(problem);
            svMachine = problem.Train(parameter);
            return DateTime.Now - starTime;
        }

        private static SVMParameter CreateSvmParameter()
        {
            return new SVMParameter
            {
                Type = SVMType.C_SVC,
                Kernel = SVMKernelType.RBF,
                C = 10,
                Gamma = 1,
                Degree = 3,
                Nu = 0.5,
                CacheSize = 10000,
                Eps = 0.001,
                Shrinking = true,
                Probability = false
            };
        }

        public void Serialize()
        {
            svMachine.SaveModel(SerializationPath);
        }

        private const string SerializationPath =  "SVM.svm";

        public bool LoadFromFile()
        {
            if (File.Exists(SerializationPath))
            {
                svMachine = SVM.LoadModel(SerializationPath);
                return svMachine != null;
            }
            return false;
        }

        private SVMProblem SvmProblem(IEnumerable<SparseNumericFeature> numericFeatures)
        {
            var problem = new SVMProblem();
            var nodes = NodeFromSparseFeature(numericFeatures);
            problem.Add(nodes.ToArray(), 0);
            return problem;
        }

        private void ScaleNewProblem(SVMProblem problem)
        {
            Scale = new double[problem.X.SelectMany(x => x).Max(x => x.Index), 2];
            foreach (var featureGroups in problem.X.SelectMany(x => x).GroupBy(x => x.Index))
            {
                var featureValue = featureGroups.Select(x => x.Value).ToArray();
                Scale[featureGroups.Key - 1, 0] = featureValue.Average();
                Scale[featureGroups.Key - 1, 1] = featureValue.CalculateStdDev();
            }
            ScaleProblem(problem);
        }

        protected void ScaleProblem(SVMProblem problem)
        {
            for (var i = 0; i < problem.X.Count; i++)
            {
                problem.X[i] = problem.X[i].Select(x =>
                {
                    x.Value = (x.Value - Scale[x.Index - 1, 0])/Scale[x.Index - 1, 1];
                    return x;
                }).ToArray();
            }
        }

        private SVMProblem CreateTrainProblem(IEnumerable<TrainingEvent> msgs)
        {
            var trainableSentences = ProccesTrainingData(msgs);
            var problem = new SVMProblem();
            foreach (var element in trainableSentences)
            {
                var nodes = NodeFromSparseFeature(element.Features);
                problem.Add(nodes, (int) element.Label);
            }
            return problem;
        }

        public SVMNode[] NodeFromSparseFeature(IEnumerable<SparseNumericFeature> features)
        {
            var nodes = features.Select(x => new SVMNode((int) x.FeatureId + 1, x.Value));
            return nodes.ToArray();
        }
    }
}