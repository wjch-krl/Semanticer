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
using System.Text.RegularExpressions;

namespace Semanticer.Classifier
{
	public class DefaultTokenizer : ITokenizer
	{
		/// <summary>
		/// Use a "\W" (non-word character) regex to split the string passed in to classify
		/// </summary>
		public static int BreakOnWordBreaks = 1;

		/// <summary>
		/// Use a "\s" (whitespace) regex to split the string pass in to classify
		/// </summary>
		public static int BreakOnWhitespace = 2;

		int tokenizerConfig = -1;
		string customTokenizerRegExp;

		#region Properties
		public int TokenizerConfig 
		{ 
			get { return tokenizerConfig; } 
			set 
			{ 
				if (value != BreakOnWordBreaks && value != BreakOnWhitespace)
					throw new ArgumentException("TokenizerConfig must be either be BREAK_ON_WORD_BREAKS or BREAK_ON_WHITESPACE");
				tokenizerConfig = value; 			
			} 
		}

		public string CustomTokenizerRegExp 
		{ 
			get { return customTokenizerRegExp; } 
			set 
			{ 
				if (value == null)
					throw new ArgumentNullException("Regular expression string must not be null.");
				customTokenizerRegExp = value; 
			} 
		}
		#endregion

		#region Constructors
		public DefaultTokenizer() : this(BreakOnWordBreaks) {}

		public DefaultTokenizer(int tokenizerConfig)
		{
			TokenizerConfig = tokenizerConfig;
		}

		public DefaultTokenizer(string regularExpression)
		{
			customTokenizerRegExp = regularExpression;
		}
		#endregion

		public virtual string[] Tokenize(string input)
		{
			string regexp;
			if (customTokenizerRegExp != null)
				regexp = CustomTokenizerRegExp;
			else if (tokenizerConfig == BreakOnWordBreaks)
				regexp = @"\W";
			else if (tokenizerConfig == BreakOnWhitespace)
				regexp = @"\s";
			else
				throw new Exception("Illegal tokenizer configuration. CustomTokenizerRegExp = null and TokenizerConfig = " + tokenizerConfig);

			if (input != null)
			{
				string[] words = Regex.Split(input, regexp, RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
				return words;
			}
		    return new string[0];
		}
	}
}