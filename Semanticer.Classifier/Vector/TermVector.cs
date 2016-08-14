using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Semanticer.Classifier.Vector
{
    [Serializable]
    public class TermVector
    {
        private readonly Dictionary<string, int> termDictionary;

        public TermVector(string[] terms, int[] values)
        {
            termDictionary = new Dictionary<string, int>();
            for (int i = 0; i < terms.Length; i++)
            {
                termDictionary.Add(terms[i],values[i]);
            }
        }

        public string[] GetTerms()
        {
            return termDictionary.Keys.ToArray();
        }

        public int[] GetValues()
        {
            return termDictionary.Values.ToArray();
        }

        public void Combine(TermVector vector)
        {
            foreach (var element in vector.termDictionary)
            {
                if (termDictionary.ContainsKey(element.Key))
                {
                    termDictionary[element.Key]+= element.Value;
                }
                else
                {
                    termDictionary.Add(element.Key,element.Value);
                }
            }
        }

        public override string ToString()
        {
            var results = new StringBuilder("{");
            foreach (var element in termDictionary)
            {
                results.Append("[");
                results.Append(element.Key);
                results.Append(", ");
                results.Append(element.Value);
                results.Append("] ");
            }
            results.Append("}");
            return results.ToString();
        }
    }
}
