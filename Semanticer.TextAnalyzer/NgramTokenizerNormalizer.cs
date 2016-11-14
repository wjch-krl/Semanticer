using System.Collections.Generic;
using Semanticer.Classifier.Common;
using Semanticer.TextAnalyzer.Utilities;
using Semanticer.TextAnalyzer.SpellChekers;
using Semanticer.Common.Utils;

namespace Semanticer.TextAnalyzer
{

	class NgramTokenizerNormalizer : NgramTokenizer
	{
	    internal string Lang { get; set; }
		internal IStopWordProvider StopWordProvider { get; set; }
	    readonly INormalizer checker;

		public NgramTokenizerNormalizer (int nGramSize) : base(nGramSize)
		{
		    checker = new LemmaNormalizer ();
		}

	    public override string [] Tokenize (string input)
		{
			string [] words = NormalizeWords (input);
			return CreateNgram (words);
		}

		string [] NormalizeWords (string input)
		{
			var words = Clear(input).SplitByWhitespaces ();
            List<string> normalized = new List<string>();
			foreach (string word in words)
			{
			    if (!StopWordProvider.IsStopWord (word)) 
			    {
			        var tmpWord = checker.Normalize (word, Lang);
			        if (!StopWordProvider.IsStopWord (word))
			        {
			            normalized.Add(tmpWord);
			        }
			    }
			}
			return normalized.ToArray();
		}

	    private string Clear(string input)
	    {
	        var cleared = input.Replace(':', ' ');
	        cleared = cleared.Replace(',', ' ');
	        cleared = cleared.Replace('.', ' ');
	        return cleared;
	    }
	}
}