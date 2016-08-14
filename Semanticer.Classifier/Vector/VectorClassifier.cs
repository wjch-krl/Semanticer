using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Vector
{
    /// <summary>
    /// Klasyfikator wykorzystujący algorytm przeszukiwania modelu przestrzeni wektorowej
    /// </summary>
    public class VectorClassifier : ClassifierBase, ITrainable
    {
        public static double DefaultVectorclassifierCutoff = 0.80d;
        private const int NumTermsInVector = 200;
        private readonly ITokenizer tokenizer;
        private readonly IStopWordProvider stopWordsProvider;
        private readonly ITermVectorStorage storage;
        private readonly IPivotWordProvider pivotWordProvider;

        public VectorClassifier() : 
            this(new DictionaryTermVectorStorage())
        {
        }
        
        public VectorClassifier(ITermVectorStorage storage): 
            this(new DefaultTokenizer(), new DefaultStopWordProvider(),new PolishPivotWordProvider(),storage)
        {}

        public VectorClassifier(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider) :
            this(tokenizer, new DefaultStopWordProvider(), pivotWordProvider, new DictionaryTermVectorStorage())
        {
        }

        public VectorClassifier(ITokenizer tokenizer, IStopWordProvider stopWordsProvider, IPivotWordProvider pivotWordProvider, ITermVectorStorage storage)
        {
            this.tokenizer = tokenizer;
            this.stopWordsProvider = stopWordsProvider;
            this.pivotWordProvider = pivotWordProvider;
            this.storage = storage;
            MatchCutoff = DefaultVectorclassifierCutoff;
        }

        public double Classify(string category, string input)
        {
            // Create a map of the word frequency from the input
            var wordFrequencies = Utilities.GetWordFrequency(input, false, tokenizer, stopWordsProvider,pivotWordProvider);
            TermVector tv = storage.GetTermVector(category);
            if (tv == null)
            {
                return 0;
            }
            int[] inputValues = GenerateTermValuesVector(tv.GetTerms(), wordFrequencies);

            return VectorUtils.CosineOfVectors(inputValues, tv.GetValues());
        }

        public bool IsMatch(string category, string input)
        {
            return (MatchCutoff < Classify(category, input));
        }

        protected int[] GenerateTermValuesVector(string[] terms, Dictionary<string, int> wordFrequencies)
        {
            var result = new int[terms.Length];
            for (int i = 0; i < terms.Length; i++)
            {
                if (wordFrequencies.ContainsKey(terms[i]))
                {
                    result[i] = wordFrequencies[terms[i]];
                }
            }
            return result;
        }

        public override IDictionary<PostMarkType, double> Classify(string input)
        {
            return new Dictionary<PostMarkType, double>
            {
                {PostMarkType.Negative, Classify(PostMarkType.Negative.ToString(),input)},
                {PostMarkType.Positive, Classify(PostMarkType.Positive.ToString(),input)},
                {PostMarkType.Neutral, Classify(PostMarkType.Neutral.ToString(),input)}
            };
        }

        public TimeSpan ReTrain(ITrainingData trainingData)
        {
            var startRime = DateTime.Now;
            var reader = trainingData.Reader;
            while (reader.HasNext())
            {
                var input = reader.ReadNextEvent();
                var wordFrequencies = Utilities.GetWordFrequency(input.GetContext(), pivotWordProvider);
                // get the numTermsInVector most used words in the input
                var mostFrequentWords = Utilities.GetMostFrequentWords(NumTermsInVector, wordFrequencies);
                var terms = mostFrequentWords.ToArray();
                Array.Sort(terms);
                var values = GenerateTermValuesVector(terms, wordFrequencies);
                var tv = new TermVector(terms, values);
                storage.AddTermVector(input.Outcome, tv);
            }
            return DateTime.Now - startRime;
        }
    }
}
