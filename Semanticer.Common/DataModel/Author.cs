using System;

namespace Semanticer.Common.DataModel
{
    public class Author
    {
        public int ID { get; set; }

        public string OrgId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Locale { get; set; }

        public char Sex { get; set; }

        public DateTime DateInsert { get; set; }

        public DateTime DateUpdate { get; set; }

        public Uri ProfileURL { get; set; }

        public Uri AvatarURL { get; set; }

        public bool IsNew { get; set; }

        public int SourceId { get; set; }

        public string FullName => string.Join(" ", FirstName, MiddleName, LastName);
        public string FullId => string.Join("_", OrgId, SourceId);
    }
}
