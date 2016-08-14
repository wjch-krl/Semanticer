using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Profil użytkownika
    /// </summary>
    public class Profile
    {
        public Profile()
        {
            Data = new ProfileData();
        }

        public bool IsUsed { get; set; }

        public int Id { get; set; }
        public int ProfileId { get; set; }
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
        public bool IsAnalyzed { get; set; }
        public bool ForceLanguage { get; set; }
        public ProfileUpdateFrequency UpdateFrequency { get; set; }
        public DateTime AnaliticsTime { get; set; }

        /// <summary>
        /// Ignorowane słowa
        /// </summary>
        public IList<string> IgnoredWords { get; set; }

        /// <summary>
        /// Ignorowani użytkownicy
        /// </summary>
        public IList<BlockedAuthor> IgnoredUsers { get; set; }

        public override string ToString()
        {
            return FriendyName ?? Name ?? OrgId;
        }

        /// <summary>
        /// Język profilu i jego analizy
        /// </summary>
        public string Language { get; set; }

        public int TradeId { get; set; }
        //   public SourceType SourceType { get{ return } }
        public LearnigAlghoritm Alghoritm { get; set; }

        /// <summary>
        /// Czy jest pobieralny
        /// </summary>
        public bool IsProfile { get; set; }

        public int AnalId { get; set; }

        public bool ShouldBeDownloaded()
        {
            return AnaliticsTime <
                   DateTime.Now.AddHours(UpdateFrequency == ProfileUpdateFrequency.Hourly ? -1 : -24);
        }

        public bool NeedToDowloadMore(Profile exciteProfile)
        {
            if (Data.GetType() != exciteProfile.Data.GetType())
            {
                return true;
                //throw new ArgumentException("Profile Data Type missmatch");
            }
            if (Data.MathCount > exciteProfile.Data.MathCount ||
                Data.ProcessedCount > exciteProfile.Data.MathCount)
            {
                return true;
            }
            var data = Data as CrawlerProfileData;
            if (data != null)
            {
                var thisData = data;
                var exciteProfileData = (CrawlerProfileData) exciteProfile.Data;
                return thisData.OnlyInDomain == exciteProfileData.OnlyInDomain &&
                       thisData.Level == exciteProfileData.Level;
            }
            //Czy trzeba więcej danych - Wtedy gdy nowy profil nie blokuje autorów, których blokował stary
            if (Extensions.IsNullOrEmpty(IgnoredUsers) && Extensions.IsNullOrEmpty(exciteProfile.IgnoredUsers))
            {
                return false;
            }
            //Jeśli conajmniej jedno nie null - pobieraj
            if (Extensions.IsNullOrEmpty(IgnoredUsers) || Extensions.IsNullOrEmpty(exciteProfile.IgnoredUsers))
            {
                return true;
            }
            return exciteProfile.IgnoredUsers.
                Any(blockedAuthor =>
                    IgnoredUsers.Count(x => x.OrgId == blockedAuthor.OrgId
                                            && x.SourceId == blockedAuthor.SourceId) == 0);
        }

        public bool HasPrivateInfo()
        {
            var data = Data as AuthorizedProfileData;
            return (data != null && string.IsNullOrEmpty(data.Login));
        }

        public int CompareProfiles(Profile exciteProfile)
        {
            if (HasPrivateInfo())
            {
                return -2;
                //Jeśli zdefiniowane dane do logowania nie współdzielić pobranych postów - treści mogą się różnić
            }
            if ((Language == exciteProfile.Language || string.IsNullOrEmpty(exciteProfile.Language)) &&
                Alghoritm == exciteProfile.Alghoritm &&
                ForceLanguage == exciteProfile.ForceLanguage)
            {
                if (Extensions.CompareCollections(IgnoredUsers, exciteProfile.IgnoredUsers) &&
                    Extensions.CompareCollections(IgnoredWords, exciteProfile.IgnoredWords))
                    return 0;
            }
            return 1;
        }

        enum ProfileComparsion
        {
            HasPrivateInfo,
            Equals,
            Diffrent,
            NeedsToDownloadMore
        }
    }
}