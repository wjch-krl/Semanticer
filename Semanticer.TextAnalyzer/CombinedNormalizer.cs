using System.Linq;
using Semanticer.TextAnalyzer.SpellChekers;

namespace Semanticer.TextAnalyzer
{
    /// <summary>
    /// Combines INormalizer objcets, durring normalization all normalizers must return same value. Otherwise non normalized word is assigned
    /// </summary>
    public class CombinedNormalizer : INormalizer
    {
        private readonly INormalizer[] normalizers;

        public CombinedNormalizer(params INormalizer[] normalizers)
        {
            this.normalizers = normalizers;
        }

        public string Normalize(string word, string lang)
        {
            var normalized = normalizers.Select(normalizer => normalizer.Normalize(word, lang));
            var distinct = normalized.Distinct().ToArray();
            return distinct.Length == 1 ? distinct[0] : word.ToLower();
        }
    }
}