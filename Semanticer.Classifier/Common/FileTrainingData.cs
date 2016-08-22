using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public class FileTrainingData: ITrainingData
    {
        private readonly ITokenizer tokenizer;

        public FileTrainingData(string path, ITokenizer tokenizer)
        { 
            this.tokenizer = tokenizer;
            Path = path;
        }

        private string Path { get; }
        public ITrainingEventReader Reader => new FileTrainReader(Path,tokenizer);
    }
}