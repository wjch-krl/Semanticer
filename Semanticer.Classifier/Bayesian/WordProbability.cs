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

namespace Semanticer.Classifier.Bayesian
{
	/// <summary>
	/// Represents the probability of a particular word.
	/// </summary>
	[Serializable]
	public class WordProbability : IComparable
	{
	    private const int Undefined = -1;

	    string word = string.Empty;
		string category = string.Empty;
		long matchingCount = Undefined;
		long nonMatchingCount = Undefined;
		double probability = ClassifierConstants.NeutralProbability;

        public string Word 
		{ 
			get { return word; } 
			set { word = value; } 
		}

		public string Category
		{
			get { return category; }
			set { category = value; }
		}
		
		public long MatchingCount 
		{ 
			get 
			{ 
				if (matchingCount == Undefined)
					throw new ApplicationException("MatchingCount has not been defined.");
				return matchingCount; 
			} 
			set 
			{ 
				matchingCount = value; 
				CalculateProbability();
			} 
		}
		
		public long NonMatchingCount
		{ 
			get 
			{ 
				if (nonMatchingCount == Undefined)
					throw new ApplicationException("NonMatchingCount has not been defined.");
				return nonMatchingCount;
			} 
			set 
			{ 
				nonMatchingCount = value; 
				CalculateProbability();
			} 
		}
		
		public double Probability 
		{ 
			get { return probability; } 
			set 
			{ 
				probability = value; 
				matchingCount = Undefined;
				nonMatchingCount = Undefined;
			} 
		}

		public WordProbability(string w, double probability)
		{
			Word = w;
			Probability = probability;
		}

		public WordProbability(string w, long matchingCount, long nonMatchingCount)
		{
			Word = w;
			MatchingCount = matchingCount;
			NonMatchingCount = nonMatchingCount;
		}

		public void RegisterMatch()
		{
			MatchingCount++;
			CalculateProbability();
		}

		public void RegisterNonMatch()
		{
			NonMatchingCount++;
			CalculateProbability();
		}

		private void CalculateProbability()
		{
			double result = ClassifierConstants.NeutralProbability;

			if (matchingCount == 0)
			{
				if (nonMatchingCount == 0)
					result = ClassifierConstants.NeutralProbability;
				else
					result = ClassifierConstants.LowerBound;
			}
			else
				result = BayesianClassifier.NormalizeSignificance(matchingCount / (double)(matchingCount + nonMatchingCount));

			probability = result;
		}

		#region IComparable Members
		public int CompareTo(object obj)
		{
			if (!(obj is WordProbability))
				throw new InvalidCastException(obj.GetType() + " is not a " + GetType());
			WordProbability rhs = (WordProbability)obj;
			
			if (Category != rhs.Category)
				return Category.CompareTo(rhs.Category);
		    if (Word != rhs.Word)
		        return Word.CompareTo(rhs.Word);
		    return 0;
		}

		public string Tostring()
		{
			return GetType() + "Word" + Word + "Probability" + Probability + "MatchingCount" + MatchingCount + "NonMatchingCount" + NonMatchingCount;
		}
		#endregion
	}
}
