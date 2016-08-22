using System.Collections.Generic;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public interface IPostSematicEvaluator
    {
        ///<summary>Przypisuje ocen� wszystkim wiadomo�ciom w zbiorze</summary>
        void Evaluate(IEnumerable<Post> posts);

        ///<summary>Przypisuje ocen� do danej wiadomo�ci</summary>
        void Evaluate(Post p);

		IDictionary<PostMarkType, double> Evaluate (string msg, string lang = null);

	}
}