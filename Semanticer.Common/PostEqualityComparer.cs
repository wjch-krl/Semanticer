using System;
using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    public class PostEqualityComparer : IEqualityComparer<Post>
    {
        public bool Equals(Post a1, Post a2)
        {
            return a1.SourceId == a2.SourceId && a1.OrgId == a2.OrgId;
        }


        public int GetHashCode(Post auth)
        {
            string hCode = String.Format("{0}_{1}",auth.SourceId, auth.OrgId);
            return hCode.GetHashCode();
        }
    }
}

