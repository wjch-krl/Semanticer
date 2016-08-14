using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.TextAnalyzer
{
    public interface IPostSematicEvaluator
    {
        ///<summary>Przypisuje ocenê wszystkim wiadomoœciom w zbiorze</summary>
        void Evaluate(IEnumerable<Post> posts);

        ///<summary>Przypisuje ocenê do danej wiadomoœci</summary>
        void Evaluate(Post p);

        /// <summary>Zwraca ocêne wiadomoœci w danym jêzyku i o danej bran¿y</summary>
        ///  <param name="msg">Treœæ wiadomoœci</param>
        /// <param name="lang">Jêzyk wiadomoœci</param>
        /// <param name="trade">Id bran¿y</param>
        double Evaluate(string msg, string lang);
    }
}