using System.Linq;
using System.IO;
using Semanticer.Classifier.Common;
using SharpEntropy;

namespace Semanticer.TextAnalyzer
{
    class ImdbFileEventReader : ITrainingEventReader
    {
        readonly ITokenizer tokenizer;
        bool hasNext;
        readonly string[] files;
        int idx;

        public ImdbFileEventReader(string path, ITokenizer tokenizer)
        {
            var trainPath = Path.Combine(path, "train");
            files = Directory.GetFiles(trainPath);
            hasNext = files.Any();
            this.tokenizer = tokenizer;
        }

        public bool HasNext()
        {
            return hasNext;
        }

        public TrainingEvent ReadNextEvent()
        {
            string fileName = files[idx];
            var imdbInfo = new ImdbFileInfo(fileName);
            var tokenized = tokenizer.Tokenize(ReadFile(fileName));
            string outCome  = imdbInfo.ToPostMarkType().ToString();
            idx++;
            hasNext = idx < files.Length;
            return new TrainingEvent(outCome, tokenized); ;
        }

        string ReadFile(string path)
        {
            var file = File.ReadAllText(path);
            var cleared = ClearText(file);
            return cleared;
        }

        internal static string ClearText(string file)
        {
            var message = file.Replace("<br />", string.Empty);
            message = message.Replace("<b>", string.Empty);
            message = message.Replace("</b>", string.Empty);
            message = message.Replace("</i>", string.Empty);
            message = message.Replace("<i>", string.Empty);
            return message;
        }
    }
}