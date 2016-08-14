using System;
using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common.Utils
{
    public static class Agregator
    {
        public static IDictionary<PostAggreagate<T>, PostAggreagate<T>> Aggreage<T>(this IEnumerable<Post> posts,
            Func<Post, PostAggreagate<T>> groupBy, Func<Post, T, T> agregate)
        {
            var resultDict = new Dictionary<PostAggreagate<T>, PostAggreagate<T>>(
                new PostAggreagateEqualityComparer<T>());
            foreach (var element in posts)
            {
                var newKey = groupBy(element);
                if (resultDict.ContainsKey(newKey))
                {
                    newKey = resultDict[newKey];
                }
                else
                {
                    resultDict.Add(newKey, newKey);
                }
                newKey.Data = agregate(element, newKey.Data);
            }
            return resultDict;
        }

        public static IDictionary<PostAggreagate<T>, List<Post>> Aggreage<T>(this IEnumerable<Post> posts,
            Func<Post, PostAggreagate<T>> groupBy)
        {
            var resultDict = new Dictionary<PostAggreagate<T>, List<Post>>(
                new PostAggreagateEqualityComparer<T>());
            foreach (var element in posts)
            {
                var newKey = groupBy(element);
                if (resultDict.ContainsKey(newKey))
                {
                    resultDict[newKey].Add(element);
                }
                else
                {
                    resultDict.Add(newKey, new List<Post>{element});
                }
            }
            return resultDict;
        }

        public static DateTime ToMonthDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }
    }
}