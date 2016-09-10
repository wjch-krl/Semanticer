using System;
using System.Linq;

namespace Semanticer.Common.DataModel
{
    public class DailyStats
    {
        private readonly HourStats[] stats;

        public DailyStats()
        {
            stats = Enumerable.Range(0,24).Select(x=>new HourStats()).ToArray();
        }

        public HourStats[] HourStats => stats;

        public void Add(DateTime tweetTime, SemanticResult semantics)
        {
            stats[tweetTime.Hour].Add(semantics);
        }
    }
}