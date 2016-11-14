using System;
using System.Linq;

namespace Semanticer.Common.DataModel
{
    [Serializable]
    public class DailyStats
    {
        public DailyStats()
        {
            HourStats = Enumerable.Range(0,24).Select(x=>new HourStats()).ToArray();
        }

        public HourStats[] HourStats { get; set; }

        public void Add(DateTime tweetTime, SemanticResult semantics)
        {
            HourStats[tweetTime.Hour].Add(semantics);
        }
    }
}