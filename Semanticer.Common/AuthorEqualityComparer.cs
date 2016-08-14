using System;
using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    public class AuthorEqualityComparer : IEqualityComparer<Author>
    {
        public bool Equals(Author a1, Author a2)
        {
            return a1.SourceId == a2.SourceId && a1.OrgId == a2.OrgId;
        }

        public int GetHashCode(Author auth)
        {
            string hCode = String.Format("{0}_{1}",auth.SourceId, auth.OrgId);
            return hCode.GetHashCode();
        }
    }
}
