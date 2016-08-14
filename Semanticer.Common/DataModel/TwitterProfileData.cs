using System;

namespace Semanticer.Common.DataModel
{
    [Serializable]
    public class TwitterProfileData : ProfileData
    {
        public ClientApiCredential Credential { get; set; }

        public long LastPostId { get; set; }

        public string[] Hashtags { get; set; }
    }
}
