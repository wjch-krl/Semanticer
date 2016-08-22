using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.TextAnalyzer
{
    public interface IPostSematicEvaluator
    {
        ///<summary>Przypisuje ocen� wszystkim wiadomo�ciom w zbiorze</summary>
        void Evaluate(IEnumerable<Post> posts);

        ///<summary>Przypisuje ocen� do danej wiadomo�ci</summary>
        void Evaluate(Post p);

        /// <summary>Zwraca oc�ne wiadomo�ci w danym j�zyku i o danej bran�y</summary>
        ///  <param name="msg">Tre�� wiadomo�ci</param>
        /// <param name="lang">J�zyk wiadomo�ci</param>
        /// <param name="trade">Id bran�y</param>
        double Evaluate(string msg, string lang);
    }
}