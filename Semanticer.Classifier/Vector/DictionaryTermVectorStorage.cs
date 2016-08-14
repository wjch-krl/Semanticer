using System.Collections.Generic;

namespace Semanticer.Classifier.Vector
{
    /// <summary>
    /// Klasa przechowująca wektory słów 
    /// </summary>
    public class DictionaryTermVectorStorage : ITermVectorStorage
    {
        readonly Dictionary<string,TermVector> storage = new Dictionary<string, TermVector>();
        public void AddTermVector(string category, TermVector termVector)
        {
            if (storage.ContainsKey(category))
            {
                storage[category].Combine(termVector);
                return;
            }
            storage.Add(category,termVector);
        }

        public TermVector GetTermVector(string category)
        {
            return storage.ContainsKey(category) ? storage[category] : null;
        }
    }
}
