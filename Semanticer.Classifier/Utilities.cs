#region Copyright (c) 2004, Ryan Whitaker
/*********************************************************************************
'
' Copyright (c) 2004 Ryan Whitaker
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' This product uses software written by the developers of NClassifier
' (http://nclassifier.sourceforge.net).  NClassifier is a .NET port of the Nick
' Lothian's Java text classification engine, Classifier4J 
' (http://classifier4j.sourceforge.net).
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'********************************************************************************/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Semanticer.Classifier
{
	public class Utilities
	{
        public static Dictionary<string, int> GetWordFrequency(string input)
		{
			return GetWordFrequency(input, false);
		}

        public static Dictionary<string, int> GetWordFrequency(string input, bool caseSensitive)
		{
			return GetWordFrequency(input, caseSensitive, new DefaultTokenizer(), new DefaultStopWordProvider(), new PolishPivotWordProvider());
		}

	    /// <summary>
	    /// Gets a Hashtable of words and integers representing the number of each word.
	    /// </summary>
	    /// <param name="input">The string to get the word frequency of.</param>
	    /// <param name="caseSensitive">True if words should be treated as separate if they have different casing.</param>
	    /// <param name="tokenizer">A instance of ITokenizer.</param>
	    /// <param name="stopWordProvider">An instance of IStopWordProvider.</param>
	    /// <param name="pivotWordProvider"></param>
	    /// <returns></returns>
	    public static Dictionary<string, int> GetWordFrequency(string input, bool caseSensitive, ITokenizer tokenizer, IStopWordProvider stopWordProvider, IPivotWordProvider pivotWordProvider)
		{
			string convertedInput = input;
			if (!caseSensitive)
				convertedInput = input.ToLower();

			string[] words = tokenizer.Tokenize(convertedInput);
			Array.Sort(words);

			string[] uniqueWords = GetUniqueWords(words);

            var result = new Dictionary<string, int>();
			foreach (string element in uniqueWords)
			{
			    if (stopWordProvider == null || (!stopWordProvider.IsStopWord(element)))
			    {
			        if (result.ContainsKey(element))
			            result[element] = result[element] + CountWords(element, words);
			        else
			            result.Add(element, CountWords(element, words));
			    }
			}
	        return result;
		}

	    /// <summary>
	    /// Gets a Hashtable of words and integers representing the number of each word.
	    /// </summary>
	    /// <param name="input">The string to get the word frequency of.</param>
	    /// <param name="caseSensitive">True if words should be treated as separate if they have different casing.</param>
	    /// <param name="tokenizer">A instance of ITokenizer.</param>
	    /// <param name="stopWordProvider">An instance of IStopWordProvider.</param>
	    /// <param name="pivotWordProvider"></param>
	    /// <returns></returns>
	    public static Dictionary<string, int> GetWordFrequency(string[] input, IPivotWordProvider pivotWordProvider)
	    {
	        Array.Sort(input);

	        string[] uniqueWords = GetUniqueWords(input);

	        var result = new Dictionary<string, int>();
	        foreach (string element in uniqueWords)
	        {
	            if (result.ContainsKey(element))
	                result[element] = result[element] + CountWords(element, input);
	            else
	                result.Add(element, CountWords(element, input));

	        }
	        return result;
	    }

	    /// <summary>
	    /// Find all unique words in an array of words.
	    /// </summary>
	    /// <param name="input">An array of strings.</param>
	    /// <returns>An array of all unique strings.  Order is not guaranteed.</returns>
	    public static string[] GetUniqueWords(string[] input)
	    {
	        if (input == null)
	            return new string[0];
	        List<string> result = new List<string>();
	        foreach (string element in input)
	        {
	            if (!result.Contains(element))
	            {
	                result.Add(element);
	            }
	        }
	        return result.ToArray();
	    }

	    /// <summary>
		/// Count how many times a word appears in an array of words.
		/// </summary>
		/// <param name="word">The word to count.</param>
		/// <param name="words">A non-null array of words.</param>
		public static int CountWords(string word, string[] words)
		{
			// find the index of one of the items in the array
			int itemIndex = Array.BinarySearch(words, word);

			// iterate backwards until we find the first match
			if (itemIndex > 0)
				while (itemIndex > 0 && words[itemIndex] == word)
					itemIndex--;

			// now itemIndex is one item before the start of the words
			int count = 0;
			while (itemIndex < words.Length && itemIndex >= 0)
			{
				if (words[itemIndex] == word)
					count++;

				itemIndex++;

				if (itemIndex < words.Length)
					if (words[itemIndex] != word)
						break;
			}

			return count;
		}

		/// <summary>
		/// Gets an array of sentences.
		/// </summary>
		/// <param name="input">A string that contains sentences.</param>
		/// <returns>An array of strings, each element containing a sentence.</returns>
		public static string[] GetSentences(string input)
		{
			if (input == null)
				return new string[0];
		    // split on a ".", a "!", a "?" followed by a space or EOL
		    // the original Java regex was (\.|!|\?)+(\s|\z)
		    string[] result = Regex.Split(input, @"(?:\.|!|\?)+(?:\s+|\z)");

		    // hacky... doing this to pass the unit tests
		    ArrayList list = new ArrayList();
		    foreach (string s in result)
		        if (s.Length > 0)
		            list.Add(s);
		    return (string[])list.ToArray(typeof(string));
		}

        public static IList<string> GetMostFrequentWords(int numTermsInVector, IDictionary<string,int> wordFrequencies)
        {
            var result = new List<string>();
            if (wordFrequencies.Count == 0)
            {
                return result;
            }
            int freq = wordFrequencies.Max(x=>x.Value);

            while (result.Count() < numTermsInVector && freq > 0)
            {
                // this is very icky
                var words = FindWordsWithFrequency(wordFrequencies, freq);
                result.AddRange(words);
                freq--;
            }

            return result;
        }

        private static IEnumerable<string> FindWordsWithFrequency(IDictionary<string, int> wordFrequencies, int? frequency)
        {
            if (wordFrequencies == null || frequency == null)
            {
                return new string[0];
            }
            return wordFrequencies.Keys.Where(word => frequency == wordFrequencies[word]).ToArray();
        }
	}
}