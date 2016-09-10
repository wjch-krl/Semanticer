using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Numeric.Svm;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;
using SharpEntropy;
using SharpEntropy.IO;

namespace Semanticer.Classifier.Textual.MaxEnt
{
    /// <summary>
    /// Klasyfikator wykorzystujący algorytm regresji logicznej (Maximum Entropy)
    /// </summary>
    public class MaxEntClassifier : ClassifierBase, ITrainable, ISerializableClassifier
    {
        private IMaximumEntropyModel model;
        private readonly ITokenizer tokenizer;
        private readonly IPivotWordProvider pivotWordProvider;
        private readonly string lang;
        private int positiveIndex;
        private int negativeIndex;
        private int neutralIndex;
        private readonly string modelPath;
        private const double Tolerance = 0.000001;

        public MaxEntClassifier(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider, string lang)
        {
            this.tokenizer = tokenizer;
            this.pivotWordProvider = pivotWordProvider;
            this.lang = lang;
            //Sciezka do modelu w zależności od języku i branży
            modelPath = $"MaxEnt_{lang}.model";
        }

        public override IDictionary<MarkType, double> Classify(string input)
        {
            var result = model.Evaluate(tokenizer.Tokenize(input));
            return new Dictionary<MarkType, double>
            {
                {MarkType.Negative, negativeIndex == -1 ? 0 : result[negativeIndex]},
                {MarkType.Positive, positiveIndex == -1 ? 0 : result[positiveIndex]},
                {MarkType.Neutral, neutralIndex == -1 ? 0 : result[neutralIndex]},
            };
        }

        public TimeSpan ReTrain(ITrainingData trainingData)
        {
            var date = DateTime.Now;
            var trainer = new GisTrainer();//PivotGisTrainer(pivotWordProvider);
            trainer.TrainModel(trainingData.Reader);
            model = new GisModel(trainer);
            positiveIndex = model.GetOutcomeIndex(MarkType.Positive.ToString());
            negativeIndex = model.GetOutcomeIndex(MarkType.Negative.ToString());
            neutralIndex = model.GetOutcomeIndex(MarkType.Neutral.ToString());
            var trainTime = DateTime.Now - date;
            return trainTime;
        }

        public new MarkType Evaluate(string input)
        {
            var result = model.Evaluate(tokenizer.Tokenize(input));
            double max = result.Max();
            if (result.CalculateStdDev() < 0.001) return MarkType.Neutral;
            if (Math.Abs(result[neutralIndex] - max) < Tolerance) return MarkType.Neutral;
            if (Math.Abs(result[positiveIndex] - max) < Tolerance) return MarkType.Positive;
            if (Math.Abs(result[negativeIndex] - max) < Tolerance) return MarkType.Negative;
            return MarkType.Neutral;
        }

        public void Serialize()
        {
            var writer = new BinaryGisModelWriter();
            writer.Persist((GisModel)model, modelPath);
        }

        public bool LoadFromFile()
        {
            if (!File.Exists(modelPath))
            {
                return false;
            }
            model = new GisModel(new BinaryGisModelReader(modelPath));
            positiveIndex = model.GetOutcomeIndex(MarkType.Positive.ToString());
            negativeIndex = model.GetOutcomeIndex(MarkType.Negative.ToString());
            neutralIndex = model.GetOutcomeIndex(MarkType.Neutral.ToString());
            return true;
        }
    }
}