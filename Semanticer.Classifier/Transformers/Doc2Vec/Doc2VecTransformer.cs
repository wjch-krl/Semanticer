using System.Collections.Generic;
using System.IO;
using System.Linq;
using Semanticer.Common.Utils;
using SemanticerDoc2VecWrapper;

namespace Semanticer.Classifier.Transformers.Doc2Vec
{
    public class Doc2VecTransformer : ITextTransformer
    {
        private readonly int skipCount;
        readonly Doc2VecWrapper doc2VecWrapper;
        public const string CorpaFileName = "C:\\mgr\\Semanticer\\aclImdb\\tweeter_corpa.txt";
        private const string FileName = "model_tweeter.doc2vec";

        public Doc2VecTransformer(Doc2VecArgs args, int skipCount = -1)
        {
            this.skipCount = skipCount;
            doc2VecWrapper = new Doc2VecWrapper(args.Dimmensions);
            if (File.Exists(FileName))
            {
                doc2VecWrapper.Load(FileName);
            }
            else
            {
                doc2VecWrapper.Train(CorpaFileName, args.CBow, args.Hs, args.Negative, args.Iterations,
                    args.Window, args.Alpha, args.Sample, args.MinCount, args.ThreadCount);
                doc2VecWrapper.Save(FileName);
            }
        }

        public IEnumerable<SparseNumericFeature> Transform(string[] words)
        {
            words = words.Take(999).ToArray();
            float[] array = doc2VecWrapper.InferDoc(words,skipCount);
            return array.Select((feature, idx) => new SparseNumericFeature
            {
                FeatureId = idx,
                Value = feature,
            });
        }

        public IEnumerable<SparseNumericFeature> Transform(string sentence)
        {
            return Transform(sentence.SplitByWhitespaces());
        }
    }
}
