using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semanticer.Common.Utils;
using SemanticerDoc2VecWrapper;

namespace Semanticer.Classifier.Transformers.Doc2Vec
{
    public class Doc2VecTransformer : ITextTransformer
    {
        private readonly int skipCount;
        readonly Doc2VecWrapper doc2VecWrapper;
        public const string CorpaFileName = "C:\\Users\\wk\\Documents\\mgr\\Semanticer\\aclImdb\\train\\doc2vec.train.corpa.txt";
        private const string FileName = "model.doc2vec";

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

    public class Doc2VecArgs
    {
        public int Dimmensions { get; set; }
        public int CBow { get; set; }
        public int Hs { get; set; }
        public int Negative { get; set; }
        public int Iterations { get; set; }
        public int Window { get; set; }
        public float Alpha { get; set; }
        public float Sample { get; set; }
        public int MinCount { get; set; }
        public int ThreadCount { get; set; }

        public Doc2VecArgs()
        {
            Dimmensions = 20;
            CBow = 0;
            Hs = 1;
            Negative = 0;
            Iterations = 15;
            Window = 10;
            Alpha = 0.025f;
            Sample = 0.0001f;
            MinCount = 3;
            ThreadCount = Environment.ProcessorCount;
        }
    }
}
