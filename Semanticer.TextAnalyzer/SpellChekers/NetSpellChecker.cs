using System.Collections.Generic;
using System.Text.RegularExpressions;
using NetSpell.SpellChecker;
using Semanticer.Common;

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
