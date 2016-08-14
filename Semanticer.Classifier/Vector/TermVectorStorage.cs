namespace Semanticer.Classifier.Vector
{
    public interface ITermVectorStorage
    {
        void AddTermVector(string category, TermVector termVector);
        TermVector GetTermVector(string category);
    }
}
