using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public abstract class AbstractDictionary : ISpellChecker
    {
        protected Regex linkRegex;
        protected Regex wordsRegex;
        protected Regex sentenceRegex;

        protected AbstractDictionary()
        {
            wordsRegex = new Regex(Patterns.WordSeparators, RegexOptions.Compiled);
            linkRegex = new Regex(Patterns.Link, RegexOptions.Compiled);
            sentenceRegex = new Regex(Patterns.Sentence, RegexOptions.Compiled);
        }

        public abstract bool Spell(string msg, string lang);

        public List<string> GetWords(Post post, bool correctSpelling = false, bool checSpelling = false)
        {
            var words = new List<string>();
            var message = linkRegex.Replace(post.Message, string.Empty);
            var lang = post.Lang;
            string[] split = wordsRegex.Split(message);

            bool foundError = FoundError(correctSpelling, checSpelling, split, lang, words);
            post.SpellCheckingStatus = GetSpellCheckingStatus(correctSpelling, checSpelling, foundError);
            return words;
        }

        public NormalizedMessage GetWords(Post post, HashSet<string> emoticonSet, bool correctSpelling = false,
            bool checSpelling = false)
        {
            var sentences = sentenceRegex.Split(post.Message).Where(x=>!string.IsNullOrWhiteSpace(x)).ToArray();
            var normalizedSentences = new NormalizedSentence[sentences.Length];
            int i = 0;
            foreach (var sentence in sentences)
            {
                var emoticons = sentence.SplitByWhitespaces().Where(emoticonSet.Contains).ToArray();
                string message = emoticons.Aggregate(sentence, 
                    (current, emoticon) => current.Replace(emoticon, string.Empty));
                var words = new List<string>();
                message = linkRegex.Replace(message, string.Empty);
                var lang = post.Lang;
                var split = wordsRegex.Split(message).Where(x => !string.IsNullOrWhiteSpace(x));

                bool foundError = FoundError(correctSpelling, checSpelling, split, lang, words);
                post.SpellCheckingStatus = GetSpellCheckingStatus(correctSpelling, checSpelling, foundError);
                normalizedSentences[i++] = new NormalizedSentence(words.ToArray(), emoticons.ToArray(),
                    message.Count(x => x == '?'), message.Count(x => x == '!'));
            }
            return new NormalizedMessage(normalizedSentences);
        }

        public abstract bool SpellWord(string word, string lang);

        protected abstract bool FoundError(bool correctSpelling, bool checSpelling, IEnumerable<string> split, string lang, List<string> words);

        private static SpellCheckingStatus GetSpellCheckingStatus(bool correctSpelling, bool checSpelling,
            bool foundError)
        {
            if (!foundError && checSpelling)
            {
                return SpellCheckingStatus.Correct;
            }
            if (correctSpelling)
            {
                return SpellCheckingStatus.Corrected;
            }
            return checSpelling ? SpellCheckingStatus.NotCorrect : SpellCheckingStatus.NotChecked;
        }
    }
}