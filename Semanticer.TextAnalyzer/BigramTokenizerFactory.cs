using Semanticer.Classifier.Common;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer
{
	public class BigramTokenizerFactory : ITokenizerFactory
	{
		public ITokenizer Create ()
		{
			return new NgramPostTokenizer ();
		}
	}
}