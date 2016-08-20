using System.Collections.Generic;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public interface IPostSematicEvaluator
    {
        ///<summary>Przypisuje ocenê wszystkim wiadomoœciom w zbiorze</summary>
        void Evaluate(IEnumerable<Post> posts);

        ///<summary>Przypisuje ocenê do danej wiadomoœci</summary>
        void Evaluate(Post p);

		IDictionary<PostMarkType, double> Evaluate (string msg, string lang = null);

	}
}