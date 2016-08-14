namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Class that represents bulletin board or its sub-boards.
    /// </summary>
    public class BulletinBoardStructure : BulletinBoardLink
    {
        public int TopicsCount
        {
            get; set;
        }

        public int SubBoardsCount
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <param name="url"> URL address of link </param>
        public BulletinBoardStructure(string url)
            : base(url) 
        {
        }
    }

   
}
