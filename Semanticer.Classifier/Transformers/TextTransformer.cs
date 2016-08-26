using System.Collections.Generic;
using Semanticer.Classifier.Common;

namespace Semanticer.Classifier.Transformers
{
    public abstract class TextTransformer : ITextTransformer
    {
        protected readonly ITokenizer tokenizer;
        protected readonly IPivotWordProvider PivotWordProvider;

        protected TextTransformer(ITokenizer tokenizer, IPivotWordProvider pivotWordProvider)
        {
            this.tokenizer = tokenizer;
            this.PivotWordProvider = pivotWordProvider;
        }

        public IEnumerable<SparseNumericFeature> Transform(string sentence)
        {
            var splited = tokenizer.Tokenize(sentence);
            return Transform(splited);
        }

        public virtual void AddAllWords(IEnumerable<string> words)
        {
        }

        public abstract IEnumerable<SparseNumericFeature> Transform(string[] words);
    }
}