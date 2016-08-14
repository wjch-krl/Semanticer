using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public interface ISpellChecker
    {
        bool Spell(string msg, string lang);
        List<string> GetWords(Post post, bool correctSpelling = false, bool checSpelling = false);
        NormalizedMessage GetWords(Post post, HashSet<string> emoticonSet, bool correctSpelling = false,
            bool checSpelling = false);
        bool SpellWord(string word, string lang);
    }
}