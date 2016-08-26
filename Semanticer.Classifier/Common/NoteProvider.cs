using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Common
{
    public class NoteProvider
    {
        private readonly IPivotWordProviderFactory pivotsFactory;
        private readonly Dictionary<string, IDictionary<string, double>> langWordDictionary;
        private readonly Dictionary<string, IDictionary<string, double>> langPhrasesDictionary;
        private Dictionary<string, double> emoticons;
        private const int DefaultTrade = 1;

        public NoteProvider(IPivotWordProviderFactory pivotsFactory)
        {
            this.pivotsFactory = pivotsFactory;
            langPhrasesDictionary = new Dictionary<string, IDictionary<string, double>>();
            langWordDictionary = new Dictionary<string, IDictionary<string, double>>();
        }

        private IEnumerable<Tuple<string, double>> IncludePivots(string lang, IList<Tuple<string, double>> bdNotes)
        {
            double multiper = 1;
            List<Tuple<string, double>> fixedNotesList = new List<Tuple<string, double>>();
            //Utworzenie obiektu odpowiedzialnego za zarz¹dzanie s³owami zmieniaj¹cymi znaczenie s¹siadów 
            var pivotWordProvider = pivotsFactory.Resolve(lang);
            foreach (var word in bdNotes)
            {
                //sprawdzamy czy dane s³owo jest zmienia wydŸwiêk s¹siada
                if (pivotWordProvider.IsPivot(word.Item1))
                {
                    //jeœli tak aktualny ustawiamy mno¿nik na mno¿nik s³owa
                    multiper = pivotWordProvider.Multiper(word.Item1);
                    //przejœcie do kolejnej iteracji
                    continue;
                }
                //Uwzglêdnienie mno¿nika poprzedniego wyrazu 
                fixedNotesList.Add(new Tuple<string, double>(word.Item1, word.Item2*multiper));//resetujemy mno¿nik
                multiper = 1;
            }
            return fixedNotesList;
        }

        public IEnumerable<Tuple<string, double>> RateEmoticons(string msg)
        {
            return msg.SplitByWhitespaces()
                   .Where(emt => emoticons.ContainsKey(emt))
                   .Select(emt => new Tuple<string, double>(emt, emoticons[emt]));
        }

        public IEnumerable<Tuple<string, double>> PrepereNotesInMemory(string msg, string lang)
        {
            if (!langPhrasesDictionary.ContainsKey(lang))
            {
                if (!PrepereLanguage(lang))
                {
                    return Enumerable.Empty<Tuple<string, double>>();
                }
            }
            var bdNotes = Notes(msg,lang);
            //return bdNotes;
            return IncludePivots(lang, bdNotes);
        }

        private IList<Tuple<string, double>> Notes(string msg, string langTrade)
        {
            var result = new List<Tuple<string, double>>();
            foreach (var phrase in langPhrasesDictionary[langTrade])
            {
                if (msg.Contains(phrase.Key))
                {
                    result.Add(new Tuple<string, double>(phrase.Key, phrase.Value));
                    msg = msg.Replace(phrase.Key, string.Empty);
                }
            }
            result.AddRange(
                msg.SplitByWhitespaces()
                    .Where(word => langWordDictionary[langTrade].ContainsKey(word))
                    .Select(word => new Tuple<string, double>(word, langWordDictionary[langTrade][word])));
            return result;
        }

        public bool PrepereLanguage(string langName)
        {
            return false;
        }

        private static Dictionary<string, double> PrepereDict(IEnumerable<LexiconWord> phrases)
        {
            var phrasesDict = new Dictionary<string, double>();
            foreach (var phrase in phrases)
            {
                if (!phrasesDict.ContainsKey(phrase.Word))
                {
                    phrasesDict.Add(phrase.Word, phrase.WordMark);
                }
                else
                {
                    phrasesDict[phrase.Word] = phrasesDict[phrase.Word] + phrase.WordMark;
                }
            }
            return phrasesDict;
        }

        public IEnumerable<Tuple<string, double>> PrepereNotesInMemory(NormalizedMessage normalizeMessage, string lang)
        {
            if (!langPhrasesDictionary.ContainsKey(lang))
            {
                if (!PrepereLanguage(lang))
                {
                    return Enumerable.Empty<Tuple<string, double>>();
                }
            }
            var bdNotes = Notes(normalizeMessage, lang);
            //return bdNotes;
            return IncludePivots(lang, bdNotes);
        }

        private IList<Tuple<string, double>> Notes(NormalizedMessage normalizeMessage, string langTrade)
        {
            return Notes(normalizeMessage.ToString(), langTrade);
        }
    }
}