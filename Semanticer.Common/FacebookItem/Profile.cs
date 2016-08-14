using System;
using System.Collections.Generic;

namespace ComarchBI.SocialIntelligence.Common.DataModel
{
    /// <summary>
    /// Profil użytkownika
    /// </summary>
    public class Profile
    {
        public bool IsUsed { get; set; }

        public int Id { get; set; }
        public int SourceId { get; set; }

        public string Name { get; set; }
        public string FriendyName { get; set; }
        public string OrgId { get; set; }
        public string ClientOrgId { get; set; }
        
        public Uri ImageUri { get; set; }
        public Uri ProfileUri { get; set; }

        public DateTime DateInsert { get; set; }
        public DateTime DateUpdate { get; set; }
        public DateTime DateProfileCreate { get; set; }
        public DateTime DateProfileUpdate { get; set; }

        public ProfileData Data { get; set; }

        /// <summary>
        /// Ignorowane słowa
        /// </summary>
        public IList<string> IgnoredWords { get; set; }
    }
}
