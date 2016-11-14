using System.IO;
using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{
    public class TnCorpaCreator
    {
        private readonly ITokenizer tokenizer;
        private readonly string trainFilePath;
        private readonly string testFilePath;

        public TnCorpaCreator(ITokenizer tokenizer,string trainFilePath, string testFilePath)
        {
            this.tokenizer = tokenizer;
            this.trainFilePath = trainFilePath;
            this.testFilePath = testFilePath;
        }

        public void CreateCorpa(string outputPath)
        {
            using (var fileWriter = new StreamWriter(outputPath))
            {
                WriteFromFile(fileWriter, trainFilePath);
                WriteFromFile(fileWriter, testFilePath);
            }
        }

        private void WriteFromFile(StreamWriter destinationWriter, string sourceFile)
        {
            using (var fileReader = TkinkNookTweeterEventReader.CreateReader(sourceFile))
            {
                while (fileReader.Read())
                {
                    var record = fileReader.GetRecord<TnTweeterCsvObject>();
                    var words = tokenizer.Tokenize(record.SentimentText);
                    var message = string.Join(" ", words);
                    destinationWriter.WriteLine($"_*{record.ItemId} {message}");
                }
            }
        }
    }
}