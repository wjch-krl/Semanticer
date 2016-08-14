using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NetSpell.SpellChecker;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class NetSpellChecker : ISpellChecker
    {
        /// <summary>
        /// Lista separatorów poszczególnych słów
        /// </summary>
        //  private static string[] separators = { " ", ".", "," };
        private readonly Regex linkRegex;
        private readonly Regex wordsRegex;
        private readonly Spelling spell;

        /// <summary>
        /// Sprawdzanie pisowni w określonym języku ?określa kulturę?
        /// </summary>
        public NetSpellChecker()
        {
            spell = new Spelling();
            wordsRegex = new Regex(Patterns.WordSeparators);
            linkRegex = new Regex(Patterns.Link);
            spell.ShowDialog = false;
            spell.IgnoreHtml = true;
        }
         
        public List<string> GetWords(Post post, bool correctSpelling = false, bool checSpelling = false)
        {
            var words = new List<string>();
            //List<string> suggestions;
            // string temp;
            var message = linkRegex.Replace(post.Message, string.Empty);
            string[] split = wordsRegex.Split(message);
            bool foundError = false;

            if (checSpelling)
            {
                foundError = spell.SpellCheck(post.Message);
            }
            foreach (var word in split.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var tmp = word;
                if (foundError && correctSpelling && spell.SpellCheck(tmp))
                {
                    spell.Suggest();
                    tmp = spell.Suggestions.Count > 0 ? spell.Suggestions[0].ToString() : tmp;
                }
                if (string.IsNullOrEmpty(tmp)) continue;
                words.Add(tmp);
            }
            if (!foundError)
            {
                post.SpellCheckingStatus = SpellCheckingStatus.Correct;
            }
            else if (correctSpelling)
            {
                post.SpellCheckingStatus = SpellCheckingStatus.Corrected;
            }
            else
            {
                post.SpellCheckingStatus = SpellCheckingStatus.NotCorrect;
            }
            return words;
        }

        public NormalizedMessage GetWords(Post post, HashSet<string> emoticonSet, bool correctSpelling = false, bool checSpelling = false)
        {
            throw new NotImplementedException();
        }

        public bool SpellWord(string word, string lang)
        {
            return spell.SpellCheck(word);
        }

        public IEnumerable<string> SpellWords(IEnumerable<string> words)
        {
            List<string> ret = new List<string>();
            foreach (var word in words)
            {
                if (spell.SpellCheck(word))
                {
                    spell.Suggest(word);
                    ret.Add(spell.Suggestions.Count > 0 ? spell.Suggestions[0].ToString() : word);
                }
                else
                {
                    ret.Add(word);
                }
            }
            return ret;
        }

        public bool Spell(string msg, string lang)
        {
            return spell.SpellCheck(msg);
        }
    }

}
