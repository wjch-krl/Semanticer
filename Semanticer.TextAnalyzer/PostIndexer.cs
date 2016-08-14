using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Logger;
using Semanticer.TextAnalyzer.SpellChekers;

namespace Semanticer.TextAnalyzer
{
    /// <summary>
    /// Klasa obslugujaca indeksacje postow, normalizowanie wyrazow i wyszukiwanie slow kluczowych
    /// </summary>
    public class PostIndexer : IClientJob
    {
        /// <summary>
        /// Database Provider dla danego indeksera
        /// </summary>
        private IPostDataProvider postsDataProvider;

        /// <summary>
        /// Event rozgłaszany po zakończeniu indeksowania postu
        /// </summary>
        public event EventHandler<DiagnosticLogElement> JobCompleted;
        /// <summary>
        /// Profil dla którego jest przeprowadzana analiza
        /// </summary>
        private Profile profile;
        /// <summary>
        /// List of words to ignore for current profile during indexing
        /// </summary>
        private HashSet<string> ignoredWords;
        /// <summary>
        /// Obiekt klasy zapisującej logi
        /// </summary>
        private LoggProvider logger;

        private IEnumerable<string> propperNames;

        HashSet<string> emoticonSet;
        private INormalizer normalizer;
        private ISpellChecker spellChecker;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PostIndexer(int profileId, IPostDataProvider postsDataProvider, ITextAnalizerDataProvider textDataProvider,
            IMainDataProvider mainDataProvider,IClientDataProvider clientDataProvider, int runId)
        {
            this.postsDataProvider = postsDataProvider;
            profile = mainDataProvider.GetProfileById(profileId);
            int langId = textDataProvider.LangId(profile.Language);
            var iwp = clientDataProvider.GetStopList(profile.Id);
            ignoredWords = new HashSet<string>(textDataProvider.StopWords(langId).Concat(iwp));
            propperNames = textDataProvider.GetProperNames();
            emoticonSet = new HashSet<string>(textDataProvider.Emoticons().Select(x => x.Word));
            normalizer = new CombinedNormalizer(
                new LemmaNormalizer(),
                NHunspellChecker.Instance);
            spellChecker = NHunspellChecker.Instance;
            logger = new LoggProvider(this, runId);
        }

        /// <summary>
        /// Metoda indeksująca podany post (istniejący już w bazie)
        /// </summary>
        /// <param name="post"> Post do zaindeksowania </param>
        public NormalizedMessage NoramalizeMessage(Post post)
        {
            if (string.IsNullOrEmpty(post.Lang))
                throw new ArgumentNullException(string.Format("Property 'Lang' in post {0} is not set", post.Id));
            var tmpMsg = post.Message;
            if (string.IsNullOrEmpty(tmpMsg))
            {
                return NormalizedMessage.Empty;
            }
            var wordsInPost = spellChecker.GetWords(post, emoticonSet, profile.Data.CorrectSpelling, true);
            Normalize(wordsInPost, post.Lang);
            post.NormalizeMessage = wordsInPost;
            
            return post.NormalizeMessage;
        }

        private void Normalize(NormalizedMessage wordsInPost, string lang)
        {
            foreach (var sentence in wordsInPost.Sentences)
            {
                sentence.Words = sentence.Words.Select(x => Normalize(x, lang)).ToArray();
            }
        }

        private string Normalize(string word, string language)
        {
            var normalized = normalizer.Normalize(word, language);
            if (!string.IsNullOrEmpty(normalized)/* && spellChecker.SpellWord(normalized,language)*/)
            {
                return normalized;
            }
            return word;
        }

        private void RaiseJobCompleted(DiagnosticLogElement args)
        {
            EventHandler<DiagnosticLogElement> handler = JobCompleted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Metoda pobierająca posty dla danego profilu, zawierająca słowa kluczowe zdefiniowane w Data tego profilu
        /// </summary>
        /// <param name="profileId"> Profil dla którego szukamy postów </param>
        /// <returns> Lista postów ze słowami kluczowymi </returns>
        public List<Post> GetPostsByKeywords(IEnumerable<string> keywords,int profileId)
        {
            List<Post> selectedPosts = new List<Post>();
            if (profile == null)
                return selectedPosts;
            List<Post> postsToCheck = postsDataProvider.GetPostsForProfile(profileId);
            if (postsToCheck.Count == 0)
                return selectedPosts;
            var enumerable = keywords as string[] ?? keywords.ToArray();
            foreach (Post post in postsToCheck)
            {
                if (string.IsNullOrEmpty(post.Lang))
                {
                    throw new ArgumentNullException(string.Format("Property 'Lang' in post {0} is not set", post.Id));
                }
                if (SearchKeywords(post.NormalizeMessage.SplitByWhitespaces(), enumerable, post.Lang))
                {
                    selectedPosts.Add(post);
                }
            }
            return selectedPosts;
        }

        /// <summary>
        /// Metoda wyszukująca podanych słów kluczowych, w danym języku,  w podanym poście
        /// </summary>
        /// <param name="words"> Post do przeszukania </param>
        /// <param name="keywords"> Szukane słowa kluczowe </param>
        /// <param name="lang"> Język posta (i slow kuczowych) </param>
        /// <returns> Czy znaleziono jakieś słowa kluczowe </returns>
        public bool SearchKeywords(IList<string> words, IEnumerable<string> keywords, string lang)
        {
            var startTime = logger.LoggStart(LoggerEventType.PostKeywordsSearchStart, string.Empty);
            bool ifFound = SearchForKeywords(words, keywords, lang);
            RaiseJobCompleted(new DiagnosticLogElement
            {
                CompletitionTime = DateTime.UtcNow - startTime,
                Found = ifFound ? 1 : 0,
                Processed = words.Count,
                JobType = LoggerEventType.PostKeywordsSearchFinished,
            });
            return ifFound;
        }

        private bool SearchForKeywords(IList<string> words, IEnumerable<string> keywords, string lang)
        {
            return keywords.Select(keyword => Normalize(keyword, lang)).
                Any(normalizedKeyword => words.
                    Any(x => x == normalizedKeyword));
        }

        /// <summary>
        /// Metoda normalizujaca wysztkie wyrazy znajdujace sie w przekazanej liscie, zgodnie ze slownika przekazanego jezyka
        /// </summary>
        /// <param name="words"> Lista slow do znormalizowania </param>
        /// <param name="lang"> Jezyk w ktorym jest przeprowadzana normalizacja </param>
        /// <returns> Lista znormalizowanych slow </returns>
        public IList<string> NormalizeWords(IList<string> words, string lang)
        {
            return words.Select(w => Normalize(w, lang)).ToList();
        }

        public int ProfileId => profile.Id;
    }
}
