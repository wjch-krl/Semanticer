using System.Text;

namespace Semanticer.TextAnalyzer.Utilities
{
    public static class PatternBuilder
    {
        public static string MissingLetterPattern(string word)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }
            builder.Append(word[0]);
            for (int index = 1; index < word.Length; index++)
            {
                var element = word[index];
                builder.Append("\\w?");
                builder.Append(element);               
            }
            return builder.ToString();
        }

        public static string RedundatLetterPattern(string word)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var element in word)
            {
                builder.AppendFormat("{0}+",element);
            }
            return builder.ToString();
        }
    }
}