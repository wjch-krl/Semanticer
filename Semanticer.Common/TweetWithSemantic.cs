using Tweetinvi.Models;

namespace Semanticer.Common
{
    public class TweetWithSemantic
    {
        public ITweet Tweet { get; set; }
        public SemanticResult Semantics { get; set; }
    }
}