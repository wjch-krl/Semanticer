using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class HourStats
    {
        private readonly Dictionary<PostMarkType, long> statsDictionary;

        public HourStats()
        {
            var enumValues = (IEnumerable<PostMarkType>) Enum.GetValues(typeof(PostMarkType));
            this.statsDictionary = enumValues.ToDictionary(x => x, x => 0L);
        }

        public void Add(SemanticResult semantics)
        {
            statsDictionary[semantics.Result]++;
        }

        public long this[PostMarkType key] => statsDictionary[key];
    }
}