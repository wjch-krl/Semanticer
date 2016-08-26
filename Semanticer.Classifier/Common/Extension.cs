using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semanticer.Common.Enums;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public static class Extension
    {
        public static PostMarkType GetMarkType(this TrainingEvent trainingEvent)
        {
           var parsed = Enum.Parse(typeof(PostMarkType), trainingEvent.Outcome);
            return (PostMarkType) parsed;
        }

    }
}
