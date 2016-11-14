using System;
using System.Collections.Generic;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Numeric.kNN;
using Semanticer.Classifier.Numeric.Svm;
using Semanticer.Classifier.Textual.Bayesian;
using Semanticer.Classifier.Textual.MaxEnt;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public class TrainableSematicEvaluator : ISematicEvaluator
    {
        private readonly string lang;
        private readonly LearnigAlghoritm alghoritm;
        private readonly IPivotWordProviderFactory pivotFactory;
        private readonly ITokenizerFactory tokenizerFactory;
        public readonly IClassifier classifier;
        private TextTransformerFactory textTransformer;
        private double PolarityMargin { get; set; }
        private double CutOff { get; set; }

        public ITokenizerFactory TokenizerFactory => tokenizerFactory;

        public IPivotWordProviderFactory PivotFactory => pivotFactory;

        public string Lang => lang;

        public TrainableSematicEvaluator(LearnigAlghoritm alghoritm, string lang, TextTransformerFactory textTransformer)
        {
            this.lang = lang;
            this.textTransformer = textTransformer;
            this.alghoritm = alghoritm;
            pivotFactory = new SimplePivotWordProviderFactory();
            tokenizerFactory = new NgramTokenizerNormalizerFactory(lang,1);
            textTransformer.Lang = lang;
            textTransformer.PivotFactory = pivotFactory;
            textTransformer.TokenizerFactory = tokenizerFactory;
            var toTrain = TrainNewClassifier();
            classifier = toTrain;
        }

        IClassifier TrainNewClassifier()
        {
            var clasifier = CreateClassifier();
            return TrainClassifier((ITrainable) clasifier);
        }

        IClassifier TrainClassifier(ITrainable clasifier)
        {
            var traingData = new ImdbFileTrainData(tokenizerFactory.Create(), ClassifierConstants.ImdbTrainDatasetPath);
            clasifier.ReTrain(traingData);
            return clasifier;
        }

        public IDictionary<MarkType, double> Evaluate(string msg)
        {
            return classifier.Classify(msg);
        }

        private IClassifier CreateClassifier()
        {
            var tokenizer = tokenizerFactory.Create();
            var pivot = pivotFactory.Resolve(lang);
            var transformer = textTransformer.CreateTextTransformer();
            IClassifier classifer;
            switch (alghoritm)
            {
                case LearnigAlghoritm.MaxEnt:
                    classifer = new MaxEntClassifier(tokenizer, pivot, lang);
                    break;
                case LearnigAlghoritm.Svm:
                    classifer = new Svm (transformer);
                    break;
                case LearnigAlghoritm.Knn:
                    classifer = new KnnClassifer(transformer);
                    break;
                default:
                    classifer = new InMemoryBayes(pivot, tokenizer, lang);
                    break;
            }
            classifer.MatchCutoff = CutOff;
            classifer.PolarityMargin = PolarityMargin;
            return classifer;
        }
    }
}