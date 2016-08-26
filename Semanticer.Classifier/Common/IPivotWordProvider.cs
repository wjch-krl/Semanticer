namespace Semanticer.Classifier.Common
{
    public interface IPivotWordProvider
    {
        bool IsPivot(string word);
        double Multiper(string word);
    }
}