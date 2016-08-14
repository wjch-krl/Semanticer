using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    public class LinkEqualityComparer : IEqualityComparer<Link>
    {

        public bool Equals(Link x, Link y)
        {
            return x.Url == y.Url;
        }

        public int GetHashCode(Link obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}
