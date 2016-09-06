using System;
using System.Collections.Generic;
using Semanticer.Classifier;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Numeric.kNN;
using Semanticer.Classifier.Numeric.Svm;
using Semanticer.Classifier.Textual.Bayesian;
using Semanticer.Classifier.Textual.MaxEnt;
using Semanticer.Classifier.Transformers;
using Semanticer.Classifier.Transformers.Doc2Vec;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public class TrainablePostSematicEvaluator : IPostSematicEvaluator
    {
        private readonly string lang;
        private readonly LearnigAlghoritm alghoritm;
        private readonly IPivotWordProviderFactory pivotFactory;
        private readonly ITokenizerFactory tokenizerFactory;
        private readonly Dictionary<string, IClassifier> classifiers;
        private double PolarityMargin { get; set; }
        private double CutOff { get; set; }


        public TrainablePostSematicEvaluator(LearnigAlghoritm alghoritm, string lang)
        {
            this.lang = lang;
            this.alghoritm = alghoritm;
            pivotFactory = new SimplePivotWordProviderFactory();
            tokenizerFactory = new NgramTokenizerNormalizerFactory(lang,1);
            classifiers = new Dictionary<string, IClassifier>();
            var toTrain = TrainNewClassifier();
            classifiers.Add(lang, toTrain);

        }

        IClassifier TrainNewClassifier()
        {
            var clasifier = CreateClassifier();
            return TrainClassifier((ITrainable) clasifier);
        }

        IClassifier TrainClassifier(ITrainable clasifier)
        {
            var traingData = new ImdbFileTrainData(tokenizerFactory.Create(), ClassifierConstants.ImdbDatasetPath);
            clasifier.ReTrain(traingData);
            return clasifier;
        }

        public IDictionary<PostMarkType, double> Evaluate(string msg, string lang = null)
        {
            lang = lang ?? this.lang;
            if (!classifiers.ContainsKey(lang))
            {
                throw new InvalidOperationException("Classifier is not train for specified language");
            }
            return classifiers[lang].Classify(msg);
        }

        private IClassifier CreateClassifier(bool forceLoad = false)
        {
            var tokenizer = tokenizerFactory.Create();
            var pivot = pivotFactory.Resolve(lang);
            IClassifier classifer;
            switch (alghoritm)
            {
                case LearnigAlghoritm.MaxEnt:
                    classifer = new MaxEntClassifier(tokenizer, pivotFactory.Resolve(lang), lang, forceLoad);
                    break;
                case LearnigAlghoritm.Svm:
                    classifer = new Svm (new Doc2VecTransformer(new Doc2VecArgs()));
                    break;
                case LearnigAlghoritm.Knn:
                    classifer = new KnnClassifer(new Doc2VecTransformer(new Doc2VecArgs()));
                    break;
                default:
                    classifer = new InMemoryBayes(pivotFactory.Resolve(lang), tokenizer, lang, forceLoad);
                    break;
            }
            classifer.MatchCutoff = CutOff;
            classifer.PolarityMargin = PolarityMargin;
            return classifer;
        }
    }
}