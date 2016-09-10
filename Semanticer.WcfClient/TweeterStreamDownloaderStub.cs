using System;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.WcfClient
{
    public class TweeterStreamDownloaderStub : ITweeterStreamDownloader
    {
        public TweetWithSemantic[] Tweets()
        {
            return new[]
            {
                new TweetWithSemantic {Semantics = new SemanticResult {Propability = 1.0, Result = PostMarkType.Negative,Text = "dupkso"} },
                new TweetWithSemantic {Semantics = new SemanticResult {Propability = 1.0, Result = PostMarkType.Positive,Text = "Kupsko"} },
                new TweetWithSemantic {Semantics = new SemanticResult {Propability = 1.0, Result = PostMarkType.Neutral,Text = "dupkso"} },
                new TweetWithSemantic {Semantics = new SemanticResult {Propability = 1.0, Result = PostMarkType.Neutral,Text = "dupkso1"} },
                new TweetWithSemantic {Semantics = new SemanticResult {Propability = 1.0, Result = PostMarkType.Neutral,Text = "duasdasdasdas asd aa pkso1"} },
            };
        }

        Random random = new Random();
        public DailyStats DailyStat()
        {
            var stats = new DailyStats();
            for(int i= 0; i < 100000; i++)
            {
                DateTime date = DateTime.Today.AddHours(random.Next(23));
                stats.Add(date,new SemanticResult
                {
                    Propability = random.NextDouble(),
                    Result = (PostMarkType)random.Next(4),
                    Text = "TEST",
                });
            }
            return stats;
        }

        public void Start()
        {
        }
    }
}