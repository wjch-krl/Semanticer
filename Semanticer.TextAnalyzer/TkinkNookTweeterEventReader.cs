using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using Semanticer.Classifier.Common;
using Semanticer.Common.Enums;
using SharpEntropy;

namespace Semanticer.TextAnalyzer
{
    public class TkinkNookTweeterEventReader : ITrainingEventReader
    {
        private readonly ITokenizer tokenizer;
        private readonly CsvReader reader;
        private bool hasNext;

        public TkinkNookTweeterEventReader(string path, ITokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
            reader = CreateReader(path);
        }

        internal static CsvReader CreateReader(string path)
        {
            var csvConf = new CsvConfiguration
            {
                Delimiter = ",",
                Quote = '"',
                HasHeaderRecord = true,
                IsHeaderCaseSensitive = false
            };
            return new CsvReader(new StreamReader(path), csvConf);
        }

        public TrainingEvent ReadNextEvent()
        {
            hasNext = reader.Read();
            var record = reader.GetRecord<TnTweeterCsvObject>();
            return CreateTrainingEvent(record);
        }

        private TrainingEvent CreateTrainingEvent(TnTweeterCsvObject record)
        {
            var message = tokenizer.Tokenize(record.SentimentText);
            var sentiment = ToMarkType(record);
            return new TrainingEvent(sentiment.ToString(), message);
        }

        private MarkType ToMarkType(TnTweeterCsvObject record)
        {
            if (record.Sentiment == 0)
            {
                return MarkType.Negative;
            }
            if (record.Sentiment ==1)
            {
                return MarkType.Positive;
            }
           return MarkType.Neutral;
        }

        public bool HasNext()
        {
            if (!hasNext)
            {
                reader.Dispose();
            }
            return hasNext;
        }

       
    }

    public class TnTweeterCsvObject
    {
        public int ItemId { get; set; }
        public int Sentiment { get; set; }
        public string SentimentSource { get; set; }
        public string SentimentText { get; set; }
    }
}