using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace Semanticer.Classifier.Common
{
    /// <summary>
    ///     Copyright (c) 2013 Kory Becker http://www.primaryobjects.com/kory-becker.aspx
    ///     Permission is hereby granted, free of charge, to any person obtaining
    ///     a copy of this software and associated documentation files (the
    ///     "Software"), to deal in the Software without restriction, including
    ///     without limitation the rights to use, copy, modify, merge, publish,
    ///     distribute, sublicense, and/or sell copies of the Software, and to
    ///     permit persons to whom the Software is furnished to do so, subject to
    ///     the following conditions:
    ///     The above copyright notice and this permission notice shall be
    ///     included in all copies or substantial portions of the Software.
    ///     Description:
    ///     Performs a TF*IDF (Term Frequency * Inverse Document Frequency) transformation on an array of documents.
    ///     Each document string is transformed into an array of doubles, cooresponding to their associated TF*IDF values.
    ///     Usage:
    ///     string[] documents = LoadYourDocuments();
    ///     double[][] inputs = TFIDF.Transform(documents);
    ///     inputs = TFIDF.Normalize(inputs);
    /// </summary>
    public class TfIdf
    {
        private readonly ITokenizer tokenizer;

        /// <summary>
        ///     Document vocabulary, containing each word's IDF value.
        /// </summary>
        private Dictionary<string, double> vocabularyIdf;

        public TfIdf(ITokenizer tokenizer,  IEnumerable<string> words)
        {
            this.tokenizer = tokenizer;
            vocabularyIdf = words.ToDictionary(x => x, y => 0.0);
        }

        /// <summary>
        ///     Transforms a list of documents into their associated TF*IDF values.
        ///     If a vocabulary does not yet exist, one will be created, based upon the documents' words.
        /// </summary>
        /// <param name="documents">string[]</param>
        /// <param name="vocabularyThreshold">Minimum number of occurences of the term within all documents</param>
        /// <returns>double[][]</returns>
        public List<List<double>> Transform(ICollection<string> documents, int vocabularyThreshold = 0)
        {
            List<List<string>> stemmedDocs;

            // Get the vocabulary and stem the documents at the same time.
            var vocabulary = GetVocabulary(documents, out stemmedDocs, vocabularyThreshold);

            if (vocabularyIdf.Count == 0)
            {
                // Calculate the IDF for each vocabulary term.
                foreach (string term in vocabulary)
                {
                    double numberOfDocsContainingTerm = stemmedDocs.Count(d => d.Contains(term));
                    vocabularyIdf[term] = Math.Log(stemmedDocs.Count/(1 + numberOfDocsContainingTerm));
                }
            }

            // Transform each document into a vector of tfidf values.
            return TransformToTfidfVectors(stemmedDocs, vocabularyIdf);
        }

        /// <summary>
        ///     Converts a list of stemmed documents (lists of stemmed words) and their associated vocabulary + idf values, into an
        ///     array of TF*IDF values.
        /// </summary>
        /// <param name="stemmedDocs">List of List of string</param>
        /// <param name="vocabularyIDF">Dictionary of string, double (term, IDF)</param>
        /// <returns>double[][]</returns>
        private List<List<double>> TransformToTfidfVectors(List<List<string>> stemmedDocs,
            Dictionary<string, double> vocabularyIDF)
        {
            // Transform each document into a vector of tfidf values.
            var vectors = new List<List<double>>();
            foreach (var doc in stemmedDocs)
            {
                var vector = new List<double>();

                foreach (var vocab in vocabularyIDF)
                {
                    // Term frequency = count how many times the term appears in this document.
                    double tf = doc.Count(d => d == vocab.Key);
                    double tfidf = tf*vocab.Value;

                    vector.Add(tfidf);
                }

                vectors.Add(vector);
            }

            return vectors.Select(v => v.ToList()).ToList();
        }

        /// <summary>
        ///     Normalizes a TF*IDF array of vectors using L2-Norm.
        ///     Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
        /// </summary>
        /// <param name="vectors">double[][]</param>
        /// <returns>double[][]</returns>
        public double[][] Normalize(IEnumerable<IEnumerable<double>> vectors)
        {
            // Normalize the vectors using L2-Norm.
            var normalizedVectors = new List<double[]>();
            foreach (var vector in vectors)
            {
                double[] normalized = Normalize(vector);
                normalizedVectors.Add(normalized);
            }

            return normalizedVectors.ToArray();
        }

        /// <summary>
        ///     Normalizes a TF*IDF vector using L2-Norm.
        ///     Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
        /// </summary>
        /// <param name="vectors">double[][]</param>
        /// <returns>double[][]</returns>
        public double[] Normalize(IEnumerable<double> vector)
        {
            IList<double> enumerable = vector as IList<double> ?? vector.ToList();
            double sumSquared = enumerable.Sum(value => value*value);

            double sqrtSumSquared = Math.Sqrt(sumSquared);

            return enumerable.Select(value => value/sqrtSumSquared).ToArray();
        }

        /// <summary>
        ///     Saves the TFIDF vocabulary to disk.
        /// </summary>
        /// <param name="filePath">File path</param>
        public void Save(string filePath = "vocabulary.dat")
        {
            // Save result to disk.
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, vocabularyIdf);
            }
        }

        /// <summary>
        ///     Loads the TFIDF vocabulary from disk.
        /// </summary>
        /// <param name="filePath">File path</param>
        public void Load(string filePath = "vocabulary.dat")
        {
            // Load from disk.
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                vocabularyIdf = (Dictionary<string, double>) formatter.Deserialize(fs);
            }
        }

        /// <summary>
        ///     Parses and tokenizes a list of documents, returning a vocabulary of words.
        /// </summary>
        /// <param name="docs">string[]</param>
        /// <param name="stemmedDocs">List of List of string</param>
        /// <returns>Vocabulary (list of strings)</returns>
        private List<string> GetVocabulary(ICollection<string> docs, out List<List<string>> stemmedDocs,
            int vocabularyThreshold)
        {
            var wordCountList = new Dictionary<string, int>();
            stemmedDocs = new List<List<string>>();

            int docIndex = 0;

            foreach (string doc in docs)
            {
                var stemmedDoc = new List<string>();

                docIndex++;

                if (docIndex%100 == 0)
                {
                    Console.WriteLine("Processing " + docIndex + "/" + docs.Count);
                }

                string[] parts2 = tokenizer.Tokenize(doc);

                var words = new List<string>();
                foreach (string part in parts2)
                {
                    // Strip non-alphanumeric characters.
                    string stripped = Regex.Replace(part, "[^a-zA-Z0-9]", "");
                    // var english = new EnglishWord(stripped);
                    string stem = stripped;
                    words.Add(stem);

                    if (stem.Length > 0)
                    {
                        // Build the word count list.
                        if (wordCountList.ContainsKey(stem))
                        {
                            wordCountList[stem]++;
                        }
                        else
                        {
                            wordCountList.Add(stem, 0);
                        }

                        stemmedDoc.Add(stem);
                    }
                }
                stemmedDocs.Add(stemmedDoc);
            }

            return wordCountList.Where(w => w.Value >= vocabularyThreshold).
                Select(item => item.Key).ToList();
        }
    }
}