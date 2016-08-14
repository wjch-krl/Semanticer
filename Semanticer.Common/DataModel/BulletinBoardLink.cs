using System.Collections.Generic;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Abstract class representing link on bulletin board
    /// </summary>
    public abstract class BulletinBoardLink : Link
    {
        /// <summary>
        /// link's title
        /// </summary>
        public string Tite{
            get;
            set;
        }

        /// <summary>
        /// List of links to next page of results 
        /// </summary>
        public List<BulletinBoardNextPage> NextPages
        {
            get;
            set;
        }

        /// <summary>
        /// Bulletin board's script type
        /// </summary>
        public ForumScriptType ScriptType
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <param name="url">URL address of link</param>
        protected BulletinBoardLink(string url)
            : base(url)
        {
            NextPages = new List<BulletinBoardNextPage>();
        }
    }
}
