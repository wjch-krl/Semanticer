namespace Semanticer.Classifier.Numeric.Svm
{
    public interface ISerializableClassifier
    {
        void Serialize();
        bool LoadFromFile();
    }
}