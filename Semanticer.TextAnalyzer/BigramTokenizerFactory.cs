using System;
using Semanticer.Classifier;
using Semanticer.TextAnalyzer.Utilities;
using Semanticer.TextAnalyzer.SpellChekers;
using Semanticer.Common.Utils;

namespace Semanticer.TextAnalyzer
{
	public class BigramTokenizerFactory : ITokenizerFactory
	{
		public ITokenizer Create ()
		{
			return new BigramPostTokenizer ();
		}
	}
}