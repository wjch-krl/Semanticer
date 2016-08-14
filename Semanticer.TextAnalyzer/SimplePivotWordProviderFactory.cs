using System.Collections.Generic;
using Semanticer.Classifier;
using Semanticer.Classifier.Common;

namespace Semanticer.TextAnalyzer
{
    public class SimplePivotWordProviderFactory : IPivotWordProviderFactory
    {
        private readonly Dictionary<string, IPivotWordProvider> providers;
        private readonly IPivotWordProvider emptyProvider;
        public SimplePivotWordProviderFactory()
        {
            providers = new Dictionary<string, IPivotWordProvider>
            {
                {"pl-PL", new PolishPivotWordProvider()},
                {"en-US", new EnglishPivotWordProvider()},
            };
            emptyProvider = new EmptyPivotWordProvider();
        }

        public IPivotWordProvider Resolve(string lang)
        {
            if (providers.ContainsKey(lang))
            {
                return providers[lang];
            }
            return emptyProvider;
        }
    }
}