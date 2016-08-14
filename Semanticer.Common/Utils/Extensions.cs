using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;

namespace Semanticer.Common.Utils
{
    public static class Extensions
    {
        public static void CombineWith(this IDictionary<Author, IList<Post>> first,
            IDictionary<Author, IList<Post>> second)
        {
            IList<Post> posts;
            foreach (var element in second)
            {
                if (first.TryGetValue(element.Key, out posts))
                {
                    posts = posts.Concat(element.Value).ToList();
                }
                else
                {
                    first.Add(element.Key, element.Value);
                }
            }
        }

        public static bool CompareCollections<T>(ICollection<T> first, ICollection<T> second)
        {
            if (Extensions.IsNullOrEmpty(first) && Extensions.IsNullOrEmpty(second))
            {
                return true;
            }
            if (Extensions.IsNullOrEmpty(first) || Extensions.IsNullOrEmpty(second))
            {
                return false;
            }
            if (first.ScrambledEquals(second))
            {
                return true;
            }
            return false;
        }

        public static void CombineWith(this IDictionary<Author, List<Post>> first,
            IDictionary<Author, List<Post>> second)
        {
            List<Post> posts;
            foreach (var element in second)
            {
                if (first.TryGetValue(element.Key, out posts))
                {
                    posts = posts.Concat(element.Value).ToList();
                }
                else
                {
                    first.Add(element.Key, element.Value);
                }
            }
        }

        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
        {
            var cnt = new Dictionary<T, int>(comparer);
            return ScrambledEquals(list1, list2, cnt);
        }

        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            return ScrambledEquals(list1, list2, cnt);
        }

        private static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2, Dictionary<T, int> cnt)
        {
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

//        private static bool ContainsAllItems<T>(this IEnumerable<T> list1, IEnumerable<T> list2, Dictionary<T, int> cnt)
//        {
//            var cnt = new Dictionary<T, int>();
//            foreach (T s in list1)
//            {
//                if (cnt.ContainsKey(s))
//                {
//                    cnt[s]++;
//                }
//                else
//                {
//                    cnt.Add(s, 1);
//                }
//            }
//        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        public static int PostsCount(this IDictionary<Author, List<Post>> first)
        {
            return first.Sum(element => element.Value.Count);
        }

        public static int PostsCount(this IDictionary<Author, IList<Post>> first)
        {
            return first.Sum(element => element.Value.Count);
        }

        public static List<Post> ToPostList(this IDictionary<Author, IList<Post>> first)
        {
            IEnumerable<Post> posts = new List<Post>();
            posts = first.Aggregate(posts, (current, element) => current.Concat(element.Value));
            return posts.ToList();
        }

        public static List<Post> ToPostList(this IDictionary<Author, List<Post>> first)
        {
            IEnumerable<Post> posts = new List<Post>();
            posts = first.Aggregate(posts, (current, element) => current.Concat(element.Value));
            return posts.ToList();
        }

        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public static string SizeSuffix(this long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            int i = 0;
            decimal dValue = value;
            while (Math.Round(dValue/1024) >= 1)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n1} {1}", dValue, SizeSuffixes[i]);
        }

        public static string[] SplitByWhitespaces(this string msg)
        {
            return msg.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        }

        public static IList<T> Shuffle<T>(this IList<T> source)
        {
            var rng = new Random();
            return source.Shuffle(rng);
        }

        public static IList<T> Shuffle<T>(this IList<T> buffer, Random rng)
        {
            var shuffle = new List<T>();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                shuffle.Add(buffer[j]);
                buffer[j] = buffer[i];
            }
            return shuffle;
        }

        public static bool SafeAdd<TKey, TElement>(this Dictionary<TKey, TElement> dict, TKey key, TElement element)
        {
            if (dict.ContainsKey(key))
            {
                return false;
            } 
            dict.Add(key,element);
            return true;
        }

        public static void SafeAddRange<TKey, TElement>(this Dictionary<TKey, TElement> dict, IEnumerable<KeyValuePair<TKey,TElement>> values)
        {
            foreach (var keyValuePair in values)
            {
                if (!dict.ContainsKey(keyValuePair.Key))
                {
                    dict.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public static Dictionary<TKey, TElement> ToDictionaryDistinct<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var result = new Dictionary<TKey, TElement>();
            foreach (var element in source)
            {
                var key = keySelector(element);
                if (!result.ContainsKey(key))
                {
                    result.Add(key, elementSelector(element));
                }
            }
            return result;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }
}