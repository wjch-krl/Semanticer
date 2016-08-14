namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Class representing next result page (for example: 2'nd page of posts in topic)
    /// </summary>
    public class BulletinBoardNextPage : Link
    {
        /// <summary>
        /// BulletinBoardLink to which current object is next page
        /// </summary>
        public BulletinBoardLink Parent
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"> URL address of link </param>
        public BulletinBoardNextPage(string url)
            : base(url)
        {
        }
    }
}
