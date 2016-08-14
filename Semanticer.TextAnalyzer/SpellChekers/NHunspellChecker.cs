using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NHunspell;
using Semanticer.Common;
using Semanticer.Common.Utils;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    /// <summary>
    /// Klasa do sprawdzania pisowni I NORMALIZACJI
    /// </summary>
    public class NHunspellChecker : AbstractDictionary , INormalizer//: IDisposable
    {
        private readonly ConcurrentDictionary<string, Hunspell> spellChekers;
     //   private ConcurrentDictionary<string, MyThes> theses; 
        private readonly Regex replicatedCharsRegex;

        public static NHunspellChecker Instance { get; set; }
        
        private NHunspellChecker()
        {
            spellChekers = new ConcurrentDictionary<string, Hunspell>();
            wordsRegex = new Regex(Patterns.WordSeparators,RegexOptions.Compiled);
            linkRegex = new Regex(Patterns.Link, RegexOptions.Compiled);
            replicatedCharsRegex = new Regex(Patterns.ReplicatedChars, RegexOptions.Compiled);
            AddLanguge("pl-PL");
           // AddLanguge("en-US");
        }

        static NHunspellChecker()
        {
            Instance = new NHunspellChecker();
        }

        /// <summary>
        /// Dodaje język do kolekcji ?określa kulturę?
        /// </summary>
        /// <param name="languageCode">Format [xx-XX], np.pl-PL</param>
        /// <param name="userDict">Słownik użytkownika (np. Nazwy własne)</param>
        public void AddLanguge(string languageCode, IEnumerable<string> userDict = null)
        {
            var spell = new Hunspell(FindLanguageFile(languageCode, "aff"), FindLanguageFile(languageCode, "dic"));
            if (userDict != null)
            {
                foreach (var element in userDict)
                {
                    spell.Add(element);
                }
            }
            spellChekers.TryAdd(languageCode, spell);
        }

        /// <summary>
        /// Wyszukiwanie ścieżki do określonego pliku pakietu językowego </summary>
        /// <param name="containString">nazwa pliku zawiera</param>
        /// <param name="fileExtension">rozszerzenie pliku</param>
        /// <returns></returns>
        private static string FindLanguageFile(string containString, string fileExtension)
        {
            string pattern = string.Format("*{0}*.{1}", containString, fileExtension);
            string directoryPath = string.Format(AppDomain.CurrentDomain.BaseDirectory + @"\Language Pack\");
            string[] results = Directory.GetFiles(directoryPath, pattern, SearchOption.TopDirectoryOnly);

            if (results.Count() > 1)
            {
                pattern = string.Format("{0}*.{1}", containString, fileExtension);
                results = Directory.GetFiles(directoryPath, pattern, SearchOption.TopDirectoryOnly);
            }

            if (results.Count() == 1)
                return results[0];

            throw new FileNotFoundException("Cannot find file that matches pattern: " + pattern);
        }

//        /// <summary>
//        /// Dokonuje podziału wypowiedzi na słowa, sprawdza i poprawia pisownie
//        /// </summary>
//        /// <param name="post">Post dla którego tekst wiadomości ma zostać przetworzona</param>
//        /// <param name="checSpelling">Czy sprawdzać pisownię</param>
//        /// <param name="correctSpelling">Czy poprawiać znalezione błędy</param>
//        /// <returns></returns>
//        public List<string> GetWords(Post post, bool correctSpelling = false, bool checSpelling = false)
//        {
//            var words = new List<string>();
//            var message = linkRegex.Replace(post.Message, string.Empty);
//            var lang = post.Lang;
//
//            string[] split = wordsRegex.Split(message);
//            if (!spellChekers.ContainsKey(lang))
//            {
//                return new List<string>(split);
//            }
//            bool foundError = false;
//            foreach (var word in split)
//            {
//                var tmp = word;
//                if (string.IsNullOrEmpty(tmp))
//                {
//                    continue;
//                }
//                var spell = spellChekers[lang];
//                lock (spell)
//                {
//                    if (checSpelling && !spell.Spell(tmp))
//                    {
//                        foundError = true;
//                        tmp = replicatedCharsRegex.Replace(tmp, "$1");
//                        if (!spell.Spell(tmp))
//                        {
//                            if (correctSpelling)
//                            {
//                                tmp = spell.Suggest(tmp).FirstOrDefault();
//                            }
//                        }
//                    }
//                }
//                words.Add(string.IsNullOrEmpty(tmp) ? word : tmp);
//            }
//            if (!foundError && checSpelling)
//            {
//                post.SpellCheckingStatus = SpellCheckingStatus.Correct;
//            }
//            else if (correctSpelling)
//            {
//                post.SpellCheckingStatus = SpellCheckingStatus.Corrected;
//            }
//            else if (checSpelling)
//            {
//                post.SpellCheckingStatus = SpellCheckingStatus.NotCorrect;
//            }
//            else
//            {
//                post.SpellCheckingStatus = SpellCheckingStatus.NotChecked;
//            }
//            return words;
//        }

        public override bool SpellWord(string word, string lang)
        {
            return spellChekers[lang].Spell(word);
        }

        protected override bool FoundError(bool correctSpelling, bool checSpelling, IEnumerable<string> split, string lang, List<string> words)
        {
            bool foundError = false;
            foreach (var word in split)
            {
                var tmp = word;
                if (string.IsNullOrEmpty(tmp))
                {
                    continue;
                }
                if (spellChekers.ContainsKey(lang))
                {
                    var spell = spellChekers[lang];
                    lock (spell)
                    {
                        if (checSpelling && !spell.Spell(tmp))
                        {
                            foundError = true;
                            if (correctSpelling)
                            {
                                if (!TryFixLetters(word, lang, out tmp))
                                {
                                    tmp = WordsHelper.BestWord(spell.Suggest(tmp), word);
                                }
                            }
                        }
                    }
                }
                words.Add(string.IsNullOrEmpty(tmp) ? word : tmp);
            }
            return foundError;
        }

        /// <summary>
        /// sprowadza słowa do formy bazowej np szybką->szybki/szybka 
        /// </summary>
        public string Normalize(string word, string language)
        {
            string normalization = null;
            if (spellChekers.ContainsKey(language))
            {
                List<string> stems;
                var spell = spellChekers[language];
                lock (spell)
                {
                    stems = spell.Stem(word);
                }
                normalization = stems.Count != 0 ? stems[stems.Count - 1].ToLower() : null;
            }
            return normalization;
        }

        public void AddProperNames(IEnumerable<string> userDict)
        {
            var userDictList = userDict as IList<string> ?? userDict.ToList();
            foreach (var spellCheker in spellChekers.Values)
            {
                lock (spellCheker)
                {
                    foreach (var element in userDictList)
                    {
                        spellCheker.Add(element);
                    }
                }
            }
        }

        public override bool Spell(string msg, string lang)
        {
            var split = msg.SplitByWhitespaces();
            return split.All(x => spellChekers[lang].Spell(x));
        }

        protected bool TryFixLetters(string word,string lang, out string tmp)
        {
            if (replicatedCharsRegex.IsMatch(word))
            {
                tmp = replicatedCharsRegex.Replace(word, "$1");
                if (Spell(tmp, lang))
                {
                    return true;
                }
            }
            tmp = null;
            return false;
        }
    }
}