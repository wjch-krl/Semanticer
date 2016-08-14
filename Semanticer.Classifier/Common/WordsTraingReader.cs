using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public class WordsTraingReader : ITrainingEventReader
    {
        private IEnumerator<LexiconWord> words;
        private bool hasNext;
        private ITrainingEventReader fileReader;

        public WordsTraingReader(ITextAnalizerDataProvider databaseProvider, ITrainingEventReader trainigFileReader, string lang, int tradeId)
        {
            words = databaseProvider.AllWords(databaseProvider.LangId(lang), tradeId).ToList().GetEnumerator();
            fileReader = trainigFileReader;
            hasNext = words.MoveNext();
        }

        public TrainingEvent ReadNextEvent()
        {
            if (hasNext)
            {
                var current = words.Current;
                hasNext = words.MoveNext(); 
                return new TrainingEvent(current.WordMark.ToPostMarkType().ToString(), new[] { current.Word });      
            }
            return fileReader.ReadNextEvent();
        }

        public bool HasNext()
        {
            return hasNext ? hasNext : fileReader.HasNext();
        }
    }
}