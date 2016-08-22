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

        public abstract bool SpellWord(string word, string lang);

        public abstract bool FoundError(bool correctSpelling, bool checSpelling, IEnumerable<string> split, string lang, List<string> words);

        protected static SpellCheckingStatus GetSpellCheckingStatus(bool correctSpelling, bool checSpelling,
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