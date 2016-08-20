using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public class FileTrainingData: ITrainingData
    {
        private readonly ITokenizer tokenizer;

        public FileTrainingData(string path, ITextAnalizerDataProvider databaseProvider, bool loadWords,ITokenizer tokenizer)
        { 
            this.tokenizer = tokenizer;
            LoadWords = loadWords;
            DatabaseProvider = databaseProvider;
            Path = path;
        }

        public string Path { get; }
        public ITrainingEventReader Reader => new FileTrainReader(Path,tokenizer);
        public ITextAnalizerDataProvider DatabaseProvider { get; }
        public bool LoadWords { get; set; }
    }
}