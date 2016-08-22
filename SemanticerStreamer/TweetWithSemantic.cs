using Tweetinvi.Models;

namespace Semanticer.Streamer
{
    public class TweetWithSemantic
    {
        public ITweet Tweet { get; set; }
        public SemanticResult Semantics { get; set; }
    }
}