using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier;
using Semanticer.Classifier.Bayesian;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.MaxEnt;
using Semanticer.Classifier.Svm;
using Semanticer.Classifier.Vector;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Logger;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer
{
    public class TrainablePostSematicEvaluator : IPostSematicEvaluator
    {
        private readonly ITextAnalizerDataProvider provider;
        private readonly LoggProvider loggProvider;
        private readonly LearnigAlghoritm alghoritm;
        private readonly IPivotWordProviderFactory pivotFactory;
        private readonly ITokenizerFactory tokenizerFactory;
        private readonly Dictionary<string, IClassifier> classifiers;
        public double PolarityMargin { get; set; }
        public double CutOff { get; set; }
        private const string LangIdFormat = "{0}_{1}";

        public TrainablePostSematicEvaluator(ITextAnalizerDataProvider provider, LoggProvider loggProvider,
            LearnigAlghoritm alghoritm,
            IPivotWordProviderFactory pivotFactory, ITokenizerFactory tokenizerFactory, double polarityMargin = 0.2,
            double cutOff = 0.1)
        {
            this.provider = provider;
            this.loggProvider = loggProvider;
            this.alghoritm = alghoritm;
            this.pivotFactory = pivotFactory;
            this.tokenizerFactory = tokenizerFactory;
            PolarityMargin = polarityMargin;
            CutOff = cutOff;
            classifiers = new Dictionary<string, IClassifier>();
        }

        public TrainablePostSematicEvaluator()
        {
        }

        public void Evaluate(IEnumerable<Post> posts)
        {
            var eventStart = loggProvider.LoggStart(LoggerEventType.PostSematicCalcutationStart, alghoritm.ToString());
            var postsPerLang = posts.GroupBy(p => string.Format(LangIdFormat, p.Lang, p.TradeId));
            int postsCount = 0;
            foreach (var element in postsPerLang)
            {
                if (!classifiers.ContainsKey(element.Key))
                {
                    foreach (var post in element)
                    {
                        post.MarkType = PostMarkType.NonSupportedLanguage;
                    }
                    continue;
                }
                var results = classifiers[element.Key].Evaluate(element.Select(x => x.NormalizeMessage).ToArray());
                int index = 0;
                foreach (var post in element)
                {
                    post.MarkType = results[index++];
                    post.Mark = -((int) post.MarkType - 2);
                    post.MarkValue = post.Mark*post.Strong;
                    postsCount++;
                }
            }
            loggProvider.LoggStop(new DiagnosticLogElement
            {
                CompletitionTime = DateTime.UtcNow - eventStart,
                Date = DateTime.Now,
                Processed = postsCount,
                JobType = LoggerEventType.PostSematicCalcutationStop,
                LogDetails = alghoritm.ToString(),
            });
        }

        public void Evaluate(Post p)
        {
            //Wybór jeżyka na podstawie postu
            string langId = string.Format(LangIdFormat, p.Lang, p.TradeId);
            if (!classifiers.ContainsKey(langId))
            {
                p.MarkType = PostMarkType.NonSupportedLanguage;
                return;
            }
            var tmp = p.NormalizeMessage.SplitByWhitespaces();
            List<PostMarkType> marks = new List<PostMarkType>();
            for (int i = 0; i < tmp.Length/10; i++)
            {
                string msg = string.Join(" ", tmp.Skip(i*10).Take(10));
                marks.Add(classifiers[langId].Evaluate(msg));
            }
            var positive = marks.Count(x => x == PostMarkType.Positive);
            var negative = marks.Count(x => x == PostMarkType.Negative);
            p.MarkType = positive > negative ? PostMarkType.Positive : PostMarkType.Negative;
            p.MarkType = positive == negative ? PostMarkType.Neutral : p.MarkType;
            //Klasyfikacja postu wybranym klasyfikatorem
//            p.MarkType = classifiers[langId].Evaluate(p.NormalizeMessage);
            p.Mark = -((int) p.MarkType - 2);
            p.MarkValue = p.Mark*(p.Strong + 1);
        }


        public double Evaluate(string msg, string lang)
        {
            if (!classifiers.ContainsKey(lang))
            {
                throw new InvalidOperationException("Classifier is not train for specified language");
            }
            return classifiers[lang].TransformPredicion(classifiers[lang].Classify(msg));
        }

        public void LoadClassifier(string lang, int trade)
        {
            string langTrade = String.Format(LangIdFormat, lang, trade);
            var eventStart = loggProvider.LoggStart(LoggerEventType.LoadClassifierStart, langTrade);
            var classifier = CreateClassifier(provider, lang, trade, true);
            if (classifiers.ContainsKey(langTrade))
            {
                classifiers[langTrade] = classifier;
            }
            else
            {
                classifiers.Add(langTrade, classifier);
            }
            loggProvider.LoggStop(new DiagnosticLogElement
            {
                CompletitionTime = DateTime.UtcNow - eventStart,
                Date = DateTime.Now,
                JobType = LoggerEventType.LoadClassifierStop,
                LogDetails = langTrade,
                Sender = alghoritm.ToString(),
            });
        }


        private IClassifier CreateClassifier(ITextAnalizerDataProvider connection, string lang, int trdId,
            bool forceLoad = false)
        {
            var tokenizer = tokenizerFactory.Create();
            string langTradeId = String.Format(LangIdFormat, lang, trdId);
            IClassifier classifer;
            switch (alghoritm)
            {
                case LearnigAlghoritm.MaxEnt:
                    classifer = new MaxEntClassifier(tokenizer, pivotFactory.Resolve(langTradeId), lang, trdId,
                        forceLoad);
                    break;
                case LearnigAlghoritm.SvmStats:
                    classifer = new SvmStatisticalClassifier(tokenizer, lang, trdId, connection, pivotFactory, false,
                        forceLoad);
                    break;
                case LearnigAlghoritm.Vector:
                    classifer = new VectorClassifier(tokenizer, pivotFactory.Resolve(langTradeId));
                    break;
                case LearnigAlghoritm.Svm:
                    classifer = new SvmClassifier(tokenizer, langTradeId);
                    break;
                default:
//                    var wordsDataSource =
//                        new PgsqlWordsDataSource(new PgsqlConncetionManager(connection.ConnectionString), langTradeId);
//                    classifer = new BayesianClassifier(wordsDataSource, tokenizer, new PostStopWordsProvider(),
//                        pivotFactory.Resolve(langTradeId), false, false);
                    classifer = new InMemoryBayes(new PolishPivotWordProvider(), trdId,provider.LangId(lang),forceLoad);
                    break;
            }
            classifer.MatchCutoff = CutOff;
            classifer.PolarityMargin = PolarityMargin;
            return classifer;
        }

        public DiagnosticLogElement LoadFromR(IDictionary<int, UsedStats> usedStats, string lang, int trdId)
        {
            string langTradeId = String.Format(LangIdFormat, lang, trdId);

            if (!classifiers.ContainsKey(langTradeId))
            {
                classifiers.Add(langTradeId, CreateClassifier(provider, lang, trdId));
            }
            var svmStatisticalClassifier = classifiers[langTradeId] as SvmStatisticalClassifier;
            DiagnosticLogElement trainResultDetail = null;
            if (svmStatisticalClassifier != null)
            {
                trainResultDetail = svmStatisticalClassifier.LoadFromR(usedStats);
            }

            return trainResultDetail;
        }

        public TimeSpan TrainClassifier(bool includeWords, string lang, int trade)
        {
            string langTradeId = String.Format(LangIdFormat, lang, trade);
            var eventStart = loggProvider.LoggStart(LoggerEventType.ClassifierTrainingStart, langTradeId);
            var traingData = new DatabaseTrainingData(provider, includeWords,
                new UnigramPostTokenizer(), lang, trade, 0.75);
            var classifier = CreateClassifier(provider, lang, trade);
            if (!(classifier is ITrainable))
            {
                throw new NotSupportedException("Can not train not trainable classifier");
            }
            var time = ((ITrainable) classifier).ReTrain(traingData);

            if (classifiers.ContainsKey(langTradeId))
            {
                classifiers[langTradeId] = classifier;
            }
            else
            {
                classifiers.Add(langTradeId, classifier);
            }
            loggProvider.LoggStop(new DiagnosticLogElement
            {
                CompletitionTime = DateTime.UtcNow - eventStart,
                Date = DateTime.Now,
                JobType = LoggerEventType.LoadClassifierStop,
                LogDetails = langTradeId,
                Sender = alghoritm.ToString(),
            });
            return time;
        }
    }
}