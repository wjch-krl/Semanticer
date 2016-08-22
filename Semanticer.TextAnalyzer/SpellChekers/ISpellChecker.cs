using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public interface ISpellChecker
    {
        bool Spell(string msg, string lang);
        bool SpellWord(string word, string lang);
    }
}