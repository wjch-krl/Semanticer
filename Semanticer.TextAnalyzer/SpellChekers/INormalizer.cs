namespace Semanticer.TextAnalyzer.SpellChekers
{
    public interface INormalizer
    {
        string Normalize(string word, string lang);
    }
}
