using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Semanticer.Common;
using Semanticer.Common.Utils;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class TreeDictionary : AbstractDictionary, ICharacterFixer
    {
        private readonly Regex replicatedCharsRegex;
        private readonly IDictionary<string, DictionaryTree> langDicts;

        public TreeDictionary()
        {
            replicatedCharsRegex = new Regex(Patterns.ReplicatedChars);
            langDicts = new Dictionary<string, DictionaryTree>();
        }

        public void LoadWords(IEnumerable<string> words, string lang, int graphOptimalizationLevel = 5)
        {
            var dict = new DictionaryTree();
            dict.LoadWords(words, graphOptimalizationLevel);
            if (langDicts.ContainsKey(lang))
            {
                langDicts[lang] = dict;
            }
            else
            {
                langDicts.Add(lang, dict);
            }
        }

        public string FixLetters(string word, string lang)
        {
            IEnumerable<string> simmilarWords;
            if (replicatedCharsRegex.IsMatch(word))
            {
                var simpleWord = RemoveRedundantChars(word);
                simmilarWords = langDicts[lang].WordsWithRedundantLetters(simpleWord);
            }
            else
            {
                simmilarWords = langDicts[lang].WordsWithLetters(word);
            }
            return WordsHelper.BestWord(simmilarWords, word);
        }

        private string RemoveRedundantChars(string input)
        {
            return replicatedCharsRegex.Replace(input, "$1");
        }

        public override bool Spell(string msg, string lang)
        {
            var split = msg.SplitByWhitespaces();
            return split.All(element => SpellWord(element,lang));
        }

        public string Correct(string misSpelled, string lang)
        {
            IEnumerable<string> propositions = langDicts[lang].CorrectSpelling(misSpelled);
            return WordsHelper.BestWord(propositions, misSpelled);
        }

        public override bool SpellWord(string word, string lang)
        {
            return langDicts[lang].Spell(word);
        }

        public override bool FoundError(bool correctSpelling, bool checSpelling, IEnumerable<string> split, string lang, List<string> words)
        {
            bool foundError = false;
            foreach (var word in split)
            {
                var tmp = word;
                if (string.IsNullOrEmpty(tmp))
                {
                    continue;
                }
                var spell = langDicts[lang];
                lock (spell)
                {
                    if (checSpelling && !spell.Spell(tmp))
                    {
                        foundError = true;
                        if (correctSpelling)
                        {
                            var fixedLetters = FixLetters(tmp, lang);
                            if (string.IsNullOrEmpty(fixedLetters))
                            {
                                tmp = WordsHelper.BestWord(spell.CorrectSpelling(tmp), tmp);
                            }
                        }
                    }
                }
                words.Add(string.IsNullOrEmpty(tmp) ? word : tmp);
            }
            return foundError;
        }
    }
}