using System;
using Semanticer.Common;

namespace Semanticer.Streamer
{
    public class DailyStats
    {
        private readonly HourStats[] stats;

        public DailyStats()
        {
            stats = new HourStats[24];
        }

        public HourStats[] HourStats => stats;

        public void Add(DateTime tweetTime, SemanticResult semantics)
        {
            stats[tweetTime.Hour].Add(semantics);
        }
    }
}