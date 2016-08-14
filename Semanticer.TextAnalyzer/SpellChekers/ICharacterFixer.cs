namespace Semanticer.TextAnalyzer.SpellChekers
{
    public interface ICharacterFixer
    {
        string FixLetters(string word,string lang);
    }
}