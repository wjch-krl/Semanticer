using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
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

        public double Evaluate(string msg, string lang)
        {
            //Poranie ocen dla poszczególnych słów w danej wiadomości
            var bdNotes = provider.PrepereNotesInMemory(msg, lang);
            //ustawienie wartości mnożnika na 1 (brak zmiany sentymentu)
            double sum = bdNotes.Sum(x=>x.Item2);
            sum = sum > 6.0 ? 6.0 : sum;
            sum = sum < -6.0 ? -6.0 : sum;
            return sum;
        }

		IDictionary<PostMarkType, double> IPostSematicEvaluator.Evaluate (string msg, string lang)
		{
		    var evaluated = Evaluate(msg, lang);
            return new Dictionary<PostMarkType, double> { {evaluated.ToPostMarkType(), evaluated} };
		}
	}
}