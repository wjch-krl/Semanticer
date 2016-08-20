using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public class SimplePostSemanticEvaluator : IPostSematicEvaluator
    {
        private readonly NoteProvider provider;

        public SimplePostSemanticEvaluator(NoteProvider provider)
        {
            this.provider = provider;
        }

        public void Evaluate(IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                Evaluate(post);
            }
        }

        public void Evaluate(Post p)
        {
            var bdNotes = provider.PrepereNotesInMemory(p.NormalizeMessage, p.Lang,p.TradeId).ToList();
            double postValue = 0;
            if (bdNotes.Any())
                postValue = bdNotes.Average(x => x.Item2) * Math.Sqrt(p.NormalizeMessage.SplitByWhitespaces().Length);
            p.MarkValue = postValue*(p.Strong+1);
            postValue = postValue > 6.0 ? 6.0 : postValue;
            postValue = postValue < -6.0 ? -6.0 : postValue;
            p.Mark = postValue;
            p.MarkType = postValue.ToPostMarkType();
        }

        public double Evaluate(string msg, string lang)
        {
            //Poranie ocen dla poszczególnych słów w danej wiadomości
            var bdNotes = provider.PrepereNotesInMemory(msg, lang,0);
            //ustawienie wartości mnożnika na 1 (brak zmiany sentymentu)
            double sum = bdNotes.Sum(x=>x.Item2);
            sum = sum > 6.0 ? 6.0 : sum;
            sum = sum < -6.0 ? -6.0 : sum;
            return sum;
        }

		IDictionary<PostMarkType, double> IPostSematicEvaluator.Evaluate (string msg, string lang)
		{
			throw new NotImplementedException ();
		}
	}
}