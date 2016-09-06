using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{

	public class NgramTokenizerNormalizerFactory : ITokenizerFactory
	{
		private readonly string lang;
	    private readonly int nGramSize;

	    public NgramTokenizerNormalizerFactory (string lang, int nGramSize = 2)
	    {
	        this.lang = lang;
	        this.nGramSize = nGramSize;
	    }

	    public ITokenizer Create ()
		{
			IStopWordProvider stopWordProvider = new CustomizableStopWordProvider ();
			return new NgramTokenizerNormalizer (nGramSize)
			{
				Lang = lang,
				StopWordProvider = stopWordProvider
			};
		}

	}
	
}