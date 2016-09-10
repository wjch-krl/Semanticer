using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Semanticer.Classifier.Common;

namespace Semanticer.Classifier.Transformers
{
    public class BagOfWordsTransformer : TextTransformer
    {
        private Dictionary<string, long> wordsDictionary;
        private long wordCount;
        public BagOfWordsTransformer(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider) 
            : base(tokenizer, pivotWordProvider)
        {
            wordsDictionary = new Dictionary<string, long>();
            wordCount++;
        }

        public override IEnumerable<SparseNumericFeature> Transform(string[] words)
        {
            foreach (var word in words.GroupBy(x=>x))
            {
                long value;
                if (wordsDictionary.TryGetValue(word.Key, out value))
                {
                    yield return new SparseNumericFeature
                    {
                        Value = (double) word.Count()/words.Length, 
                        FeatureId = value,
                    };
                }
                else
                {
                    long id = Interlocked.Increment(ref wordCount);
                    wordsDictionary.Add(word.Key,id);
                    yield return new SparseNumericFeature
                    {
                        Value = (double)word.Count() / words.Length,
                        FeatureId = id,
                    };
                }
            }
        }
    }
}