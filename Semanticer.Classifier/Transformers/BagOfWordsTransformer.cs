using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;

namespace Semanticer.Classifier.Transformers
{
    public class BagOfWordsTransformer : TextTransformer
    {
        private Dictionary<string, WordDictValue> wordsDictionary;

        public BagOfWordsTransformer(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider) 
            : base(tokenizer, pivotWordProvider)
        {
        }

        private Dictionary<string, WordDictValue> CreateWordDictionary(IEnumerable<string> words)
        {
            long id = 0;
            return words.GroupBy(x => x).ToDictionary(x => x.Key, x =>
                new WordDictValue
                {
                    Count = x.Count(),
                    Id = id++,
                });
        }

        public override void AddAllWords(IEnumerable<string> words)
        {
            wordsDictionary = CreateWordDictionary(words);
        }

        public override IEnumerable<SparseNumericFeature> Transform(string[] words)
        {
            Validate();
            foreach (var word in words.GroupBy(x=>x))
            {
                WordDictValue value;
                if (wordsDictionary.TryGetValue(word.Key, out value))
                {
                    yield return new SparseNumericFeature
                    {
                        Value = (double)word.Count()/words.Length ,//* value.Count,
                        FeatureId = value.Id,
                    };
                }
            }
        }

        private void Validate()
        {
            if (wordsDictionary == null)
            {
                throw new ArgumentNullException("wordsDictionary");
            }
        }

        private class WordDictValue
        {
            public int Count { get; set; }
            public long Id { get; set; }
        }
    }
}