using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class HourStats
    {
        private readonly Dictionary<MarkType, long> statsDictionary;

        public HourStats()
        {
            var enumValues = (IEnumerable<MarkType>) Enum.GetValues(typeof(MarkType));
            this.statsDictionary = enumValues.ToDictionary(x => x, x => 0L);
        }

        public void Add(SemanticResult semantics)
        {
            statsDictionary[semantics.Result]++;
        }

        public long this[MarkType key] => statsDictionary[key];
    }
}