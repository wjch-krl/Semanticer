using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;
using SharpEntropy;
using SharpEntropy.IO;

namespace Semanticer.Classifier.MaxEnt
{
    /// <summary>
    /// Klasyfikator wykorzystujący algorytm regresji logicznej (Maximum Entropy)
    /// </summary>
    public class MaxEntClassifier : ClassifierBase, ITrainable
    {
        private IMaximumEntropyModel model;
        private readonly ITokenizer tokenizer;
        private readonly IPivotWordProvider pivotWordProvider;
        private readonly string lang;
        private readonly int tradeId;
        private int positiveIndex;
        private int negativeIndex;
        private int neutralIndex;
        private readonly string modelPath;
        private const double Tolerance = 0.000001;

        public MaxEntClassifier(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider, string lang, int tradeId,
            bool loadClassifier)
        {
            this.tokenizer = tokenizer;
            this.pivotWordProvider = pivotWordProvider;
            this.lang = lang;
            this.tradeId = tradeId;
            //Sciezka do modelu w zależności od języku i branży
            modelPath = string.Format("MaxEnt_{0}_{1}.model", lang, tradeId);
            if (loadClassifier)
            {
                //Ładowanie zapisanego modeleu
                model = new GisModel(new BinaryGisModelReader(modelPath));
                positiveIndex = model.GetOutcomeIndex(PostMarkType.Positive.ToString());
                negativeIndex = model.GetOutcomeIndex(PostMarkType.Negative.ToString());
                neutralIndex = model.GetOutcomeIndex(PostMarkType.Neutral.ToString());
            }
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            var result = model.Evaluate(tokenizer.Tokenize(input));
            return new Dictionary<PostMarkType, double>
            {
                {PostMarkType.Negative, negativeIndex == -1 ? 0 : result[negativeIndex]},
                {PostMarkType.Positive, positiveIndex == -1 ? 0 : result[positiveIndex]},
                {PostMarkType.Neutral, neutralIndex == -1 ? 0 : result[neutralIndex]},
            };
        }

        public TimeSpan ReTrain(ITrainingData trainingData)
        {
            var date = DateTime.Now;
            var trainer = new PivotGisTrainer(pivotWordProvider);
            trainer.TrainModel(trainingData.Reader);
            model = new GisModel(trainer);
            positiveIndex = model.GetOutcomeIndex(PostMarkType.Positive.ToString());
            negativeIndex = model.GetOutcomeIndex(PostMarkType.Negative.ToString());
            neutralIndex = model.GetOutcomeIndex(PostMarkType.Neutral.ToString());
            var trainTime = DateTime.Now - date;
            var writer = new BinaryGisModelWriter();
            writer.Persist((GisModel) model, modelPath);
            return trainTime;
        }

        public double Classify(string category, string input)
        {
            return model.Evaluate(tokenizer.Tokenize(input))[model.GetOutcomeIndex(category)];
        }

        public new PostMarkType Evaluate(string input)
        {
            var result = model.Evaluate(tokenizer.Tokenize(input));
            double max = result.Max();
            if (result.CalculateStdDev() < 0.001) return PostMarkType.Neutral;
            if (Math.Abs(result[neutralIndex] - max) < Tolerance) return PostMarkType.Neutral;
            if (Math.Abs(result[positiveIndex] - max) < Tolerance) return PostMarkType.Positive;
            if (Math.Abs(result[negativeIndex] - max) < Tolerance) return PostMarkType.Negative;
            return PostMarkType.Neutral;
        }
    }
}