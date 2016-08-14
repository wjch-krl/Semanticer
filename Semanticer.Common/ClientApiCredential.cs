using Semanticer.Common.Enums;

namespace Semanticer.Common
{
    public class ClientApiCredential
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string TokenSecret { get; set;}
        public string Token { get; set; }

        public SourceType SourceType { get; set; }
    }
}
