using System.Collections.Generic;

namespace Semanticer.Common.DataModel
{
    public class BulletinBoardTopic : BulletinBoardLink
    {
        /// <summary>
        /// 
        /// </summary>
        public HashSet<int> TopicPosts
        {
            get;
            set;
        }

        /// <summary>
        /// Ilość postów w temacie
        /// </summary>
        public int PostsCount
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"> Adres URL linku</param>
        public BulletinBoardTopic(string url)
            : base(url)
        {
            TopicPosts = new HashSet<int>();
        }
    }
}