using System;
using System.Collections.Generic;
using System.Linq;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Numeric.Svm
{
    public abstract class SvmClassifierBase : ClassifierBase, ITrainable
    {
        protected readonly string TradeLang;
        protected SVMModel Classifier;
        protected Dictionary<string, int> WordDict;
        protected string ModelPath;
        protected string DictPath;
        protected string TrainFilePath;
        protected double[,] Scale;

        protected SvmClassifierBase(ITokenizer tokenizer, string langCode, bool useRegresion)
        {
            ModelPath = $"Svm_{langCode}.model";
            TrainFilePath = $"SvmTrainData_{langCode}.ds";
            DictPath = $"Svm_{langCode}.dict";
            Tokenizer = tokenizer;
            TradeLang = langCode;
            MatchCutoff = 0.6;
        }

        protected ITokenizer Tokenizer;

        public Dictionary<PostMarkType, double>[] Classify(ICollection<string> input)
        {
            var porob = Transform(input);
            var problem = SvmProblem(porob);
            ScaleProblem(problem);

            var predicted = problem.Predict(Classifier);
            return predicted.Select(PredicionDictionary).ToArray();
        }

        protected static SVMProblem SvmProblem(List<List<double>> porob)
        {
            var problem = new SVMProblem();
            foreach (var element in porob)
            {
                problem.Add(element.Where(x => x != 0.0).Select((t, i) => new SVMNode(i + 1, t)).ToArray(), 0);
            }
            return problem;
        }

        private Dictionary<PostMarkType, double> PredicionDictionary(double prediction)
        {
            return new Dictionary<PostMarkType, double>
            {
                {PostMarkType.Positive, (int) prediction == (int) PostMarkType.Positive ? 1 : 0},
                {PostMarkType.Negative, (int) prediction == (int) PostMarkType.Negative ? 1 : 0},
                {PostMarkType.Neutral, (int) prediction == (int) PostMarkType.Neutral ? 1 : 0},
            };
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            var porob = Transform(new[] {input});
            var problem = SvmProblem(porob);
            ScaleProblem(problem);
            var x = problem.X[0];
            var prediction = Classifier.Predict(x);
            return PredicionDictionary(prediction);
        }

        public override PostMarkType[] Evaluate(string[] input)
        {
            var porob = Transform(input);
            var problem = SvmProblem(porob);
            ScaleProblem(problem);
            var pred = problem.Predict(Classifier);
//            List<double[]> tmp;
//            var pred1 = problem.PredictProbability(Classifier,out tmp);
//            if (!pred.SequenceEqual(pred1))
//            {
//                Debug.Write("dd");
//            }
            return pred.Select(x => (PostMarkType)x).ToArray();
        }

        public override PostMarkType Evaluate(string input)
        {
            var porob = Transform(new[] {input});
            var problem = SvmProblem(porob);
            ScaleProblem(problem);
            var x = problem.X[0];
           // var prediction = Classifier.Predict(x);
            double[] prob;
            var prediction = Classifier.PredictProbability(x, out prob);
            return (PostMarkType) prediction;
        }

        public TimeSpan ReTrain(ITrainingData data)
        {
            SVMParameter parameter;

            SvmTrainigData svmTraingData;
            if ((svmTraingData = data as SvmTrainigData) != null)
            {
                parameter = svmTraingData.SvmParameter;
            }
            else
            {
                parameter = new SVMParameter
                {
                    Kernel = SVMKernelType.RBF,
                    C = 0.1767767,
                    Gamma = 0.3535534,
                };
            }
            DateTime starTime = DateTime.Now;
            var msgs = new List<KeyValuePair<string, double>>();
            var reader = data.Reader;
            while (reader.HasNext())
            {
                var input = reader.ReadNextEvent();
                var key = string.Join(" ", input.GetContext());
                msgs.Add(new KeyValuePair<string, double>(key, (int) Enum.Parse(typeof (PostMarkType), input.Outcome)));
            }
            var problem = CreateTrainProblem(msgs);
            ScaleNewProblem(problem);
            
            Classifier = problem.Train(parameter);
            //SVM.SaveModel(Classifier, ModelPath);
            //SerializeHelper();
            return DateTime.Now - starTime;
        }

        protected abstract SVMProblem CreateTrainProblem(List<KeyValuePair<string, double>> trainMessages);

        protected abstract void SerializeHelper();
        protected abstract List<List<double>> Transform(string[] wordsMartrix);

        protected List<List<double>> Transform(IEnumerable<string> documents)
        {
            return Transform(documents.ToArray());
        }

        private void ScaleNewProblem(SVMProblem problem)
        {
            Scale = new double[problem.X.SelectMany(x=>x).Max(x=>x.Index), 2];
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
            for (int i = 0; i < problem.X.Count; i++)
            {
                problem.X[i] = problem.X[i].Select(x =>
                {
                    x.Value = (x.Value - Scale[x.Index - 1, 0])/Scale[x.Index - 1, 1];
                    return x;
                }).ToArray();
            }
        }
    }
}