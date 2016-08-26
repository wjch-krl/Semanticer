using System.Globalization;
using System.Linq;
using NTextCat;

namespace Semanticer.TextAnalyzer
{
    public class LanguageIdentifier
    {
        private readonly RankedLanguageIdentifier identifier;
        private readonly ILookup<string, CultureInfo> cultures; 
        public LanguageIdentifier()
        {
            var factory = new RankedLanguageIdentifierFactory();
            identifier = factory.Load("LanguageModels\\Wiki82.profile.xml");
            cultures = CultureInfo
                .GetCultures(CultureTypes.NeutralCultures).ToLookup(x => x.TwoLetterISOLanguageName, x => x);
        }

        public string DetectLanguage(string text)
        {
            var languages = identifier.Identify(text);
            var mostCertainLanguage = languages.FirstOrDefault();
            if (mostCertainLanguage != null && mostCertainLanguage.Item2 > 0.5 && cultures.Contains(mostCertainLanguage.Item1.Iso639_2T))
            {
                var cultureInfo = cultures[mostCertainLanguage.Item1.Iso639_2T].First();
                string lang = CultureInfo.CreateSpecificCulture(cultureInfo.Name).Name;
                return lang;
            }
            return null;
        }
    }
}
