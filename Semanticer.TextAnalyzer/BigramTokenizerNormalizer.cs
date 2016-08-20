using System;
using Semanticer.Classifier;
using Semanticer.TextAnalyzer.Utilities;
using Semanticer.TextAnalyzer.SpellChekers;
using Semanticer.Common.Utils;

namespace Semanticer.TextAnalyzer
{

	class BigramTokenizerNormalizer : BigramPostTokenizer
	{
		internal string Lang { get; set; }
		internal IStopWordProvider StopWordProvider { get; set; }
		INormalizer checker;

		public BigramTokenizerNormalizer ()
		{
			checker = new LemmaNormalizer ();
		}

		public override string [] Tokenize (string input)
		{
			string [] words = NormalizeWords (input);
			return CreateBigram (words);
		}

		string [] NormalizeWords (string input)
		{
			var words = input.SplitByWhitespaces ();
			for (int i = 0; i < words.Length; i++)
			{
				var word = words [i];
				if (!StopWordProvider.IsStopWord (word)) 
				{
					word = checker.Normalize (word, Lang);
					if (!StopWordProvider.IsStopWord (word))
					{
						words [i] = word;
					}
				}
			}
			return words;
		}
	}
}