using System;

namespace Semanticer.Classifier.Transformers.Doc2Vec
{
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