using System;
using System.Collections.Generic;
using System.Linq;

namespace Semanticer.Common.Utils
{
    public static class StatisticalExtensions
    {
        public static double GeometricMean(this IEnumerable<double> values)
        {
            double combined = 1;
            int count = 0;
            foreach (var element in values)
            {
                combined *= element;
                count ++;
            }
            return Math.Pow(combined, 1.0/count);
        }

        public static double CalculateStdDev(this IEnumerable<double> values)
        {
            double ret = 0;
            var valuesList = values as double[] ?? values.ToArray();
            if (valuesList.Length > 0)
            {
                double avg = valuesList.Average();
                double sum = valuesList.Sum(d => Math.Pow(d - avg, 2));
                ret = Math.Sqrt((sum)/(valuesList.Length - 1));
            }
            return ret;
        }

        public static double HarmonicMean(this IEnumerable<double> values)
        {
            double combined = 1;
            int count = 0;
            foreach (var element in values)
            {
                combined += 1/element;
                count++;
            }
            return count/combined;
        }

        public static double Median(this IEnumerable<double> list)
        {
            List<double> orderedList = list
                .OrderBy(numbers => numbers)
                .ToList();

            int listSize = orderedList.Count;
            double result;

            if (listSize%2 == 0) // even
            {
                int midIndex = listSize/2;
                result = ((orderedList.ElementAt(midIndex - 1) +
                           orderedList.ElementAt(midIndex))/2);
            }
            else // odd
            {
                double element = (double) listSize/2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);

                result = orderedList.ElementAt((int) (element - 1));
            }

            return result;
        }

        public static IEnumerable<double> Modes(this IEnumerable<double> list)
        {
            var modesList = list
                .GroupBy(values => values)
                .Select(valueCluster =>
                    new
                    {
                        Value = valueCluster.Key,
                        Occurrence = valueCluster.Count(),
                    })
                .ToList();

            int maxOccurrence = modesList
                .Max(g => g.Occurrence);

            return modesList
                .Where(x => x.Occurrence == maxOccurrence && maxOccurrence > 1) // Thanks Rui!
                .Select(x => x.Value);
        }

        //Range
    }
}