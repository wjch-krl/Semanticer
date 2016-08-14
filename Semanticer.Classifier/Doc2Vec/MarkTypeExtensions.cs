using System.Collections.Generic;
using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Doc2Vec
{
    public static class MarkTypeExtensions
    {
        public static double WeightedAvg(this IEnumerable<PostMarkType> markTypes)
        {
            double sum =0 ;
            double weightsSum = 0;
            foreach (var postMarkType in markTypes)
            {
                if (postMarkType == PostMarkType.Neutral)
                {
                    weightsSum += 1.0;
                } 
                else if (postMarkType == PostMarkType.Positive)
                {
                    weightsSum += 2.0;
                    sum += 2.0;
                }
                else if (postMarkType == PostMarkType.Negative)
                {
                    weightsSum += 2.0;
                    sum -= 2.0;
                }
            }
            return sum/weightsSum;
        }

        public static PostMarkType WeightedAvg1(this IEnumerable<PostMarkType> markTypes)
        {
            double sum = 0;
            double weightsSum = 0;
            foreach (var postMarkType in markTypes)
            {
                if (postMarkType == PostMarkType.Neutral)
                {
                    weightsSum += 1.0;
                }
                else if (postMarkType == PostMarkType.Positive)
                {
                    weightsSum += 2.0;
                    sum += 2.0;
                }
                else if (postMarkType == PostMarkType.Negative)
                {
                    weightsSum += 2.0;
                    sum -= 2.0;
                }
            }
            var value = sum / weightsSum;
            if (value < 0.0)
            {
                return PostMarkType.Negative;
            }
            if (value > 0.5)
            {
                return PostMarkType.Positive;
            }
            return PostMarkType.Neutral;
        }
    }
}