using System;
using Semanticer.Common.Enums;
using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    public static class Extension
    {
        public static MarkType GetMarkType(this TrainingEvent trainingEvent)
        {
           var parsed = Enum.Parse(typeof(MarkType), trainingEvent.Outcome);
            return (MarkType) parsed;
        }

    }
}
