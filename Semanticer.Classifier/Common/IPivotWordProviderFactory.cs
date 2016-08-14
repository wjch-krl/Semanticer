namespace Semanticer.Classifier.Common
{
    public interface IPivotWordProviderFactory
    {
        IPivotWordProvider Resolve(string lang);
    }
}