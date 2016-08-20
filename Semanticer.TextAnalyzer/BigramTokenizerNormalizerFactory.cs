using System;
using Semanticer.Classifier;
using Semanticer.TextAnalyzer.Utilities;
using Semanticer.TextAnalyzer.SpellChekers;
using Semanticer.Common.Utils;

namespace Semanticer.TextAnalyzer
{

	public class BigramTokenizerNormalizerFactory : ITokenizerFactory
	{
		private string lang;

		public BigramTokenizerNormalizerFactory (string lang)
		{
			this.lang = lang;
		}

		public ITokenizer Create ()
		{
			IStopWordProvider stopWordProvider = new DefaultStopWordProvider ();
			return new BigramTokenizerNormalizer 
			{
				Lang = lang,
				StopWordProvider = stopWordProvider
			};
		}

	}
	
}