using System.Collections.Generic;
using Semanticer.Classifier.Common;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class DatabaseNormalizer : INormalizer
    {
        private readonly ITextAnalizerDataProvider provider;
        private readonly IDictionary<string, int> langDict;

        public DatabaseNormalizer(ITextAnalizerDataProvider provider)
        {
            this.provider = provider;
            langDict = provider.LangugaesDictionary();
        }

        public string Normalize(string word, string lang)
        {
            if (langDict.ContainsKey(lang))
            {
                return WordsHelper.BestWord(provider.NormalizeWord(word, langDict[lang]),word);
            }
            return null;
        }
    }
}
