using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{

	public class BigramTokenizerNormalizerFactory : ITokenizerFactory
	{
		private readonly string lang;

		public BigramTokenizerNormalizerFactory (string lang)
		{
			this.lang = lang;
		}

		public ITokenizer Create ()
		{
			IStopWordProvider stopWordProvider = new CustomizableStopWordProvider ();
			return new BigramTokenizerNormalizer 
			{
				Lang = lang,
				StopWordProvider = stopWordProvider
			};
		}

	}
	
}