using System;
using Tweetinvi.Models;

namespace Semanticer.Common
{
    [Serializable]
    public class TweetWithSemantic
    {
        public TweetWithSemantic(ITweet tweet, SemanticResult semantics)
        {
            Semantics = semantics;
            TweetLocalCreationDate = tweet.TweetLocalCreationDate;
            Language = tweet.Language.ToString();
            CreatedBy = tweet.CreatedBy.Name;
            
        }

        public TweetWithSemantic()
        {
        }

        public DateTime TweetLocalCreationDate { get; set; }
        public string Language { get; set; }
        public string CreatedBy { get; set; }
        public SemanticResult Semantics { get; set; }
    }
}