using System.IO;
using System.Linq;
using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{
    public class ImdbFileMerger
    {
        private string[] files;
        private ITokenizer normalizer;

        public ImdbFileMerger(params string[] paths)
        {
            files = paths.SelectMany(Directory.GetFiles).ToArray();
            normalizer = new NgramTokenizerNormalizerFactory("en-US",1).Create();
        }

        public void MergeInto(string path)
        {
            using (var fileWriter = new StreamWriter(path))
            {
                int i = 0;
                foreach (var filePath in files)
                {
                    string message = string.Join(" ", File.ReadAllLines(filePath));
                    message = ImdbFileEventReader.ClearText(message);
                    var words = normalizer.Tokenize(message);
                    message = string.Join(" ", words);
                    fileWriter.WriteLine($"_*{i++} {message}");
                }
            }
        }
    }
}