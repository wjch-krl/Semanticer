using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;
using Semanticer.Common.Utils;

namespace Semanticer.Classifier.Common
{
    public class NoteProvider
    {
        private ITextAnalizerDataProvider provider;
        private IPivotWordProviderFactory pivotsFactory;
        private Dictionary<string, IDictionary<string, double>> langWordDictionary;
        private Dictionary<string, IDictionary<string, double>> langPhrasesDictionary;
        private Dictionary<string, double> emoticons;
        private const int DefaultTrade = 1;

        public NoteProvider(ITextAnalizerDataProvider provider, IPivotWordProviderFactory pivotsFactory)
        {
            this.provider = provider;
            this.pivotsFactory = pivotsFactory;
            langPhrasesDictionary = new Dictionary<string, IDictionary<string, double>>();
            langWordDictionary = new Dictionary<string, IDictionary<string, double>>();
            emoticons = PrepereDict(provider.Emoticons());
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

        public IEnumerable<Tuple<string, double>> PrepereNotesInMemory(string msg, string lang,int tradeId)
        {
            string langTrade = string.Format("{0}_{1}", lang, tradeId);
            if (!langPhrasesDictionary.ContainsKey(langTrade))
            {
                if (!PrepereLanguage(lang, tradeId))
                {
                    return Enumerable.Empty<Tuple<string, double>>();
                }
            }
            var bdNotes = Notes(msg, langTrade);
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

        public bool PrepereLanguage(string langName, int tradeId, bool useDefault = false)
        {
            int lang = provider.LangId(langName);
            int tmpTrade = useDefault ? DefaultTrade : tradeId;
            var words = provider.AllWords(lang, tmpTrade);
            var phrases = provider.Phrases(lang, tmpTrade);
            var phrasesDict = PrepereDict(phrases);
            var wordDict = PrepereDict(words);
            if (wordDict.Count == 0 || phrasesDict.Count == 0)
            {
                return (!useDefault) && PrepereLanguage(langName, tradeId, true);
            }
            string langTradeId = string.Format("{0}_{1}", langName, tradeId);
            langPhrasesDictionary.Add(langTradeId, phrasesDict);
            langWordDictionary.Add(langTradeId, wordDict);
            return true;
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

        public IEnumerable<Tuple<string, double>> PrepereNotesInMemory(NormalizedMessage normalizeMessage, string lang, int tradeId)
        {
            string langTrade = string.Format("{0}_{1}", lang, tradeId);
            if (!langPhrasesDictionary.ContainsKey(langTrade))
            {
                if (!PrepereLanguage(lang, tradeId))
                {
                    return Enumerable.Empty<Tuple<string, double>>();
                }
            }
            var bdNotes = Notes(normalizeMessage, langTrade);
            //return bdNotes;
            return IncludePivots(lang, bdNotes);
        }

        private IList<Tuple<string, double>> Notes(NormalizedMessage normalizeMessage, string langTrade)
        {
            return Notes(normalizeMessage.ToString(), langTrade);
        }
    }
}