using System;
using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    public class BlockedAuthorEqualityComparer : IEqualityComparer<BlockedAuthor>
    {
        public bool Equals(BlockedAuthor a1, BlockedAuthor a2)
        {
            return a1.SourceId == a2.SourceId && a1.OrgId == a2.OrgId;
        }

        public int GetHashCode(BlockedAuthor auth)
        {
            string hCode = String.Format("{0}_{1}", auth.SourceId, auth.OrgId);
            return hCode.GetHashCode();
        }
    }
}