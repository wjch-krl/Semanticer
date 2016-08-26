using System;
using System.Collections.Concurrent;
using LemmaSharp;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class LemmaNormalizer : INormalizer
    {
        private readonly ConcurrentDictionary<string, Lemmatizer> lemmatizers;

        public LemmaNormalizer()
        {
            lemmatizers = new ConcurrentDictionary<string, Lemmatizer>();
        }

        public string Normalize(string word, string lang)
        {
            string lemma;
            if (lemmatizers.ContainsKey(lang))
            {
                //jeśli dla danego języka przeprowadzana została normalizacja -> normalizacja
                lemma = lemmatizers[lang].Lemmatize(word);
            }
            else
            {
                LanguagePrebuilt languagePrebuilt;
                //Sprawdzenie czy dany język jest obsługiwany
                if (ParseLanguage(lang, out languagePrebuilt))
                {
                    var lemmatizer = new LemmatizerPrebuiltCompact(languagePrebuilt);
                    //załadowanie modelu językowego i dodanie go do puli
                    lemmatizers.TryAdd(lang, lemmatizer);
                    //normalizacja
                    lemma = lemmatizer.Lemmatize(word);
                }
                else
                {
                    return null;
                }
            }
            //zmiana liter na małe
            return lemma.ToLower();
        }

        private static bool ParseLanguage(string lang, out LanguagePrebuilt languagePrebuilt)
        {
            // var culture = CultureInfo.GetCultureInfo(lang.Replace("_", "-"));
            var split = lang.Split(new[] {'_', '-'}, StringSplitOptions.RemoveEmptyEntries);
            lang = split.Length == 2 ? split[0] : split[1];
            switch (lang)
            {
                case "en":
                    languagePrebuilt = LanguagePrebuilt.English;
                    return true;
                case "pl":
                    languagePrebuilt = LanguagePrebuilt.Polish;
                    return true;
                case "fr":
                    languagePrebuilt = LanguagePrebuilt.French;
                    return true;
                case "de":
                    languagePrebuilt = LanguagePrebuilt.German;
                    return true;
                case "es":
                    languagePrebuilt = LanguagePrebuilt.Spanish;
                    return true;
                case "bg":
                    languagePrebuilt = LanguagePrebuilt.Bulgarian;
                    return true;
                case "it":
                    languagePrebuilt = LanguagePrebuilt.Italian;
                    return true;
                case "cs":
                    languagePrebuilt = LanguagePrebuilt.Czech;
                    return true;
                case "et":
                    languagePrebuilt = LanguagePrebuilt.Estonian;
                    return true;
                case "hu":
                    languagePrebuilt = LanguagePrebuilt.Hungarian;
                    return true;
                case "mk":
                    languagePrebuilt = LanguagePrebuilt.Macedonian;
                    return true;
                case "fa":
                    languagePrebuilt = LanguagePrebuilt.Persian;
                    return true;
                case "ro":
                    languagePrebuilt = LanguagePrebuilt.Romanian;
                    return true;
                case "ru":
                    languagePrebuilt = LanguagePrebuilt.Russian;
                    return true;
                case "sr":
                    languagePrebuilt = LanguagePrebuilt.Serbian;
                    return true;
                case "sk":
                    languagePrebuilt = LanguagePrebuilt.Slovak;
                    return true;
                case "sl":
                    languagePrebuilt = LanguagePrebuilt.Slovene;
                    return true;
                case "uk":
                    languagePrebuilt = LanguagePrebuilt.Ukrainian;
                    return true;
            }
            languagePrebuilt = default(LanguagePrebuilt);
            return false;
        }
    }
}