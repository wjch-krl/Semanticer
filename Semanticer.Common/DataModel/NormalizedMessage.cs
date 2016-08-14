using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Semanticer.Common.DataModel
{
    public class NormalizedMessage : IEnumerable<string>
    {
        public bool IsEmpty { get; set; }

        public NormalizedMessage()
        {
            IsEmpty = true;
            Sentences = new NormalizedSentence[0];
        }

        public NormalizedMessage(NormalizedSentence[] sentences) : this()
        {
            IsEmpty = sentences.All(x => x.IsEmpty);
            this.Sentences = sentences;
        }

        public NormalizedSentence[] Sentences { get; set; }
        public static NormalizedMessage Empty => new NormalizedMessage();

        public string SimpleNormalizeMessage()
        {
            if (IsEmpty)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var sentence in Sentences)
            {
                sb.Append(sentence);
            }
            return sb.ToString();
        }

        public string Serialize()
        {
            if (IsEmpty)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var sentence in Sentences)
            {
                sb.AppendFormat("{0}\n",sentence.Serialize());
            }
            return sb.ToString();
        }

        public static NormalizedMessage Deserialize(string normalizeMessage)
        {
            if (string.IsNullOrEmpty(normalizeMessage))
            {
                return Empty;
            }
            var senstences = normalizeMessage.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            var normalizedSentences = new NormalizedSentence[senstences.Length];
            int i = 0;
            foreach (var senstence in senstences)
            {
                normalizedSentences[i++] = NormalizedSentence.Parse(senstence);
            }
            return new NormalizedMessage(normalizedSentences);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return Sentences.SelectMany(x => x.Words).GetEnumerator();
        }

        public override string ToString()
        {
            return SimpleNormalizeMessage();
        }

        public string[] SplitByWhitespaces()
        {
            return IsEmpty ? new string[0] : Sentences.SelectMany(x => x.Words).ToArray();
        }

        public bool Contains(string key)
        {
            return Sentences.Select(x => string.Join(" ", x.Words)).Any(x => x == key);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<string>)this).GetEnumerator();
        }
    }
}