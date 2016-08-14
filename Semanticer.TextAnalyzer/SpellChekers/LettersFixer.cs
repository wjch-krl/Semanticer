using System.Linq;
using System.Text.RegularExpressions;
using Semanticer.Classifier.Common;
using Semanticer.Common;
using Semanticer.TextAnalyzer.Utilities;

namespace Semanticer.TextAnalyzer.SpellChekers
{
    public class LettersFixer : ICharacterFixer
    {
        private readonly ITextAnalizerDataProvider databaseProvider;
        private Regex replicatedCharsRegex;

        public LettersFixer(ITextAnalizerDataProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
            replicatedCharsRegex = new Regex(Patterns.ReplicatedChars);
        }

        public string FixLetters(string word, string lang)
        {
            var pattern = replicatedCharsRegex.IsMatch(word)
                ? PatternBuilder.RedundatLetterPattern(RemoveRedundantChars(word))
                : PatternBuilder.MissingLetterPattern(word);
            var simmilarWords = databaseProvider.WordsMatchingPattern(pattern).ToList();
            return WordsHelper.BestWord(simmilarWords, word);       
        }

        private string RemoveRedundantChars(string input)
        {
            return replicatedCharsRegex.Replace(input, "$1");
        }
    }
}