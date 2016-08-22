using System;
using Semanticer.Classifier;
using Semanticer.Classifier.Common;
using SharpEntropy;

namespace Semanticer.TextAnalyzer
{
    class ImdbFileTrainData : ITrainingData
    {
        private readonly ITokenizer tokenizer;
        private readonly string path;

        public ITrainingEventReader Reader => new ImdbFileEventReader(path, tokenizer);

        public ImdbFileTrainData(ITokenizer tokenizer, string path)
        {
            this.path = path;
            this.tokenizer = tokenizer;
        }
    }
}