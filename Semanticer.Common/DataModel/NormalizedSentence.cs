using System;
using System.Linq;
using System.Text.RegularExpressions;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    public class NormalizedSentence
    {
        private static readonly Regex _isInNewFormat;
        static NormalizedSentence()
        {
            _isInNewFormat = new Regex(Patterns.NormalizedSentenceDeparse);
        }

        public NormalizedSentence(string[] words, string[] emoticons, int questionMarks,int excleamtors)
        {
            Words = words;
            Emoticons = emoticons;
            Excleamtors = excleamtors;
            QuestionMarks = questionMarks;
            this.IsEmpty = words.Length + emoticons.Length + excleamtors + questionMarks == 0;
        }

        public NormalizedSentence(string[] words)
        {
            Words = words;
            Emoticons = new string[0];
            this.IsEmpty = words.Length == 0;
        }

        public NormalizedSentence()
        {
            IsEmpty = true;
            Words = new string[0];
            Emoticons = new string[0];
        }

        public string[] Words { get; set; }
        public string[] Emoticons { get; set; }
        public int Excleamtors { get; set; }
        public int QuestionMarks { get; set; }
        public bool IsEmpty { get; set; }

        private string ToString(string format)
        {
            return String.Format(format,
                string.Join(" ", Words),
                string.Join(" ", Emoticons),
                string.Join(" ", Enumerable.Repeat("!", Excleamtors)),
                string.Join(" ", Enumerable.Repeat("?", QuestionMarks))); 
        }

        public override string ToString()
        {
            return ToString("{0} {1} {2} {3}");
        }

        public string Serialize()
        {
            return ToString("|| {0} ||## {1} ##&& {2} {3} &&");
        }

        public static NormalizedSentence Parse(string senstence)
        {
            var math = _isInNewFormat.Match(senstence);
            if (math.Success)
            {
                var message = math.Groups[1].Value;
                var emoticons = math.Groups[2].Value;
                var interpunction = math.Groups[3].Value;
                return new NormalizedSentence(message.SplitByWhitespaces(),
                    emoticons.SplitByWhitespaces(), interpunction.Count(x => x == '?'), interpunction.Count(x => x == '!'));
            }
            return new NormalizedSentence(senstence.SplitByWhitespaces());
        }
    }
}