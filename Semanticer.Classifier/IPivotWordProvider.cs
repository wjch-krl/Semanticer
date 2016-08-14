namespace Semanticer.Classifier
{
    public interface IPivotWordProvider
    {
        bool IsPivot(string word);
        double Multiper(string word);
    }
}