using System.IO;
using System.Linq;
using Semanticer.Common.Utils;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public class FileTrainReader : ITrainingEventReader
    {
        private readonly ITokenizer tokenizer;
        private StreamReader reader;
        private string nextLine;

        public FileTrainReader(string path, ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
            reader = new StreamReader(path);
            nextLine = reader.ReadLine();
        }

        public TrainingEvent ReadNextEvent()
        {
            var currentLine = nextLine;
            nextLine = reader.ReadLine();
            if (nextLine == null)
            {
                reader.Close();
            }
            var split = currentLine.SplitByWhitespaces();
            var splitted = tokenizer.Tokenize(string.Join(" ", split.Skip(1)));
            return new TrainingEvent(split[0], splitted);
        }

        public bool HasNext()
        {
            return nextLine != null;
        }
    }
}