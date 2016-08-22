using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;
using SharpEntropy;

namespace Semanticer.Classifier.Bayesian
{
    public class InMemoryBayes :ClassifierBase, ITrainable
    {
        private IDictionary<string, WordOccurance> wordsOccurances;
        private readonly IPivotWordProvider pivotWordProvider;
        private readonly int tradeId;
        private readonly int langId;

        public InMemoryBayes(IPivotWordProvider pivotWordProvider, int tradeId, int langId, bool forceLoad)
        {
            this.pivotWordProvider = pivotWordProvider;
            this.tradeId = tradeId;
            this.langId = langId;
            if (!LoadFromFile() && forceLoad)
            {
                throw new FileNotFoundException("Cant find model file");
            }
        }

        public void Serialize()
        {
            var path = SerializationPath();
            DictionarySerializer.Serialize(wordsOccurances,path);
        }

        private string SerializationPath()
        {
            return string.Format("InMemoryBayes-{0}-{1}.xml", langId, tradeId);
        }

        public bool LoadFromFile()
        {
            var path = SerializationPath();
            if (File.Exists(path))
            {
                wordsOccurances = DictionarySerializer.Deserialize<string, WordOccurance>(path);
                return true;
            }
            return false;
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            var dict = CreatePropabilityDict();
            var pivorRange = 0;
            double multiper = 1.0;
            foreach (var word in input.SplitByWhitespaces().
                Where(x => wordsOccurances.ContainsKey(x)))
            {
                if (pivotWordProvider.IsPivot(word))
                {
                    pivorRange = 2;
                    multiper = pivotWordProvider.Multiper(word);
                }
                if (pivorRange-- <= 0)
                {
                    multiper = 1.0;
                }
                dict[PostMarkType.Positive].Add(GetWordProbality(word, PostMarkType.Positive, multiper));
                dict[PostMarkType.Negative].Add(GetWordProbality(word, PostMarkType.Negative, multiper));
                dict[PostMarkType.Neutral].Add(GetWordProbality(word, PostMarkType.Neutral, multiper));
            }
            return CalculateOverallProbabilities(dict);
        }

        private IDictionary<PostMarkType, double> CalculateOverallProbabilities(Dictionary<PostMarkType, List<double>> dict)
        {
            return dict.ToDictionary(x => x.Key, y => CalculateOverallProbability(y.Value));
        }

        private double CalculateOverallProbability(List<double> wordPropabilities)
        {
            const double zero = 0.00000001;
            double denominatorPart = 0.0;
            double numerator = 0.0;
            foreach (var probability in wordPropabilities)
            {
                if (Math.Abs(denominatorPart) < zero)
                    denominatorPart = (1 - probability);
                else
                    denominatorPart = denominatorPart * (1 - probability);

                if (Math.Abs(numerator) < zero)
                    numerator = probability;
                else
                    numerator = numerator * probability;
            }
            double denominator = numerator + denominatorPart;
            return numerator / denominator;
        }

        private double GetWordProbality(string word, PostMarkType mark, double multiper)
        {
            var result = CalculateWordPropability(word, mark)*multiper;
            return BayesianClassifier.NormalizeSignificance(result);
        }

        private double CalculateWordPropability(string word, PostMarkType mark)
        {
            var wordOccurance = wordsOccurances[word];
            double result;
            var matchingCount = wordOccurance.MatchCount(mark);
            var nonMatchingCount = wordOccurance.NonMatchCount(mark);
            if (matchingCount == 0)
            {
                result = nonMatchingCount == 0 ? ClassifierConstants.NeutralProbability : ClassifierConstants.LowerBound;
            }
            else
            {
                result = matchingCount/(double) (matchingCount + nonMatchingCount);
            }
            return result;
        }

        private static Dictionary<PostMarkType, List<double>> CreatePropabilityDict()
        {
            return new Dictionary<PostMarkType, List<double>>
            {
                {PostMarkType.Positive, new List<double>()},
                {PostMarkType.Negative, new List<double>()},
                {PostMarkType.Neutral, new List<double>()},
            };
        }

        public TimeSpan ReTrain(ITrainingData trainingData)
        {
            var sw = Stopwatch.StartNew();
            wordsOccurances = new Dictionary<string, WordOccurance>();
            if (trainingData.LoadWords)
            {
                LoadWords(trainingData);
            }
            LoadSentences(trainingData.Reader);
            sw.Stop();
            Serialize();
            return sw.Elapsed;
        }

        private void LoadSentences(ITrainingEventReader reader)
        {
            while (reader.HasNext())
            {
                var pivorRange = 0;
                TrainingEvent trainSentence = reader.ReadNextEvent();
                PostMarkType mark = (PostMarkType) Enum.Parse(typeof (PostMarkType), trainSentence.Outcome);
                foreach (var word in trainSentence.GetContext())
                {
                    var multiper = Multiper(word,  ref pivorRange);
                    TeachWord(word, multiper, mark);
                }
            }
        }

        private void TeachWord(string word, double multiper, PostMarkType mark)
        {
            if (!wordsOccurances.ContainsKey(word))
            {
                wordsOccurances.Add(word, new WordOccurance());
            }
            if (multiper > 0)
            {
                wordsOccurances[word][mark]++;
            }
            else
            {
                wordsOccurances[word][mark]--;
            }
        }

        private double Multiper(string word, ref int pivorRange)
        {
            double multiper = 1.0;
            if (pivotWordProvider.IsPivot(word))
            {
                pivorRange = 2;
                multiper = multiper * pivotWordProvider.Multiper(word);
            }
            if (pivorRange-- <= 0)
            {
                multiper = 1.0;
            }
            return multiper;
        }

        private void LoadWords(ITrainingData trainingData)
        {
            var words = trainingData.DatabaseProvider.AllWords(langId,tradeId).Where(x=> Math.Abs(x.WordMark) > 1.5 );
            foreach (var lexiconWord in words)
            {
                TeachWord(lexiconWord.Word,1,lexiconWord.MarkType);
            }
        }


        public class WordOccurance
        {
            public int PositiveCount { get; set; }
            public int NegativeCount { get; set; }
            public int NeutralCount { get; set; }

            public int this[PostMarkType postMarkType]
            {
                get
                {
                    return MatchCount(postMarkType);
                }
                set
                {
                    if (postMarkType == PostMarkType.Positive)
                    {
                        PositiveCount = value;
                    }
                    if (postMarkType == PostMarkType.Negative)
                    {
                         NegativeCount = value; 
                    }
                    if (postMarkType == PostMarkType.Neutral)
                    {
                        NeutralCount = value; 
                    }
                }
            }

            public int MatchCount(PostMarkType postMarkType)
            {
                if (postMarkType == PostMarkType.Positive)
                {
                    return PositiveCount;
                }
                if (postMarkType == PostMarkType.Negative)
                {
                    return NegativeCount;
                }
                if (postMarkType == PostMarkType.Neutral)
                {
                    return NeutralCount;
                }
                throw new KeyNotFoundException("Invalid marktype");
            }

            public int NonMatchCount(PostMarkType postMarkType)
            {
                if (postMarkType == PostMarkType.Positive)
                {
                    return NegativeCount;
                }
                if (postMarkType == PostMarkType.Negative)
                {
                    return PositiveCount;
                }
                if (postMarkType == PostMarkType.Neutral)
                {
                    return NegativeCount + PositiveCount;
                }
                throw new KeyNotFoundException("Invalid marktype");
            }
        }

    }
}