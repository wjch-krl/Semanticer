using System.Collections.Generic;

namespace Semanticer.Common.DataModel
{
    public class BulletinBoardTopicWithPosts : BulletinBoardLink
    {
        /// <summary>
        /// Collection of posts associated to authors.
        /// </summary>
        public Dictionary<Author, List<Post>> TopicPosts
        {
            get;
            set;
        }

        /// <summary>
        /// Number of posts in topic
        /// </summary>
        public int PostsCount
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"> URL address of link </param>
        public BulletinBoardTopicWithPosts(string url)
            : base(url)
        {
            TopicPosts = new Dictionary<Author, List<Post>>(new AuthorEqualityComparer());
        }
    }
}
