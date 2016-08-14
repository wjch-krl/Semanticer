using System;
using System.Collections.Generic;
using System.Linq;
using Semanticer.Common.DataModel;

namespace Semanticer.Common.Utils
{
    public static class PostsAggregationsExtensions
    {
        public static IEnumerable<PostAggreagate<Tuple<string, int, int>>> WordCloudAggegator(
            this IEnumerable<Post> posts, int count, HashSet<string> skipWords)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<Tuple<string, int, int>>(post));
            foreach (var group in grouped)
            {
                var cloud = group.Value
                    .SelectMany(x => x.NormalizeMessage.SplitByWhitespaces())
                    .Where(x => !skipWords.Contains(x))
                    .GroupBy(x => x)
                    .Select(x => new {key = x.Key, value = x.Count()})
                    .OrderByDescending(x => x.value)
                    .Take(count);
                foreach (var word in cloud)
                {
                    yield return new PostAggreagate<Tuple<string, int, int>>(group.Key)
                    {
                        Data = new Tuple<string, int, int>(word.key, word.value, word.value)
                    }; 
                }
            }
        }

        public static IEnumerable<PostAggreagate<Tuple<int, int, int, int, int, int>>> GroupPostsByTimeAggreagator(
            this IEnumerable<Post> posts)
        {
            var grouped = posts.Aggreage(post => 
                new PostAggreagate<Tuple<int, int, int, int, int, int>>(post));
            foreach (var postPair in grouped)
            {
                int posLikes = 0; //Iloœæ polubieñ postów
                int posShares = 0; //Iloœæ ponownych udostêpnieñ postów
                int posCount = 0; //Iloœæ postów

                int comLikes = 0; //Iloœæ polubieñ komentarzy
                int comShares = 0; //Iloœæ ponownych udostêpnieñ komentarzy
                int comCount = 0; //Iloœæ komentarzy

                foreach (var pos in postPair.Value)
                {
                    //post
                    if (pos.Level == 0)
                    {
                        posLikes += pos.Strong;
                        posShares += pos.Shares;
                        posCount++;
                    }
                    //komentarz 
                    if (pos.Level > 0)
                    {
                        comLikes += pos.Strong;
                        comShares += pos.Shares;
                        comCount++;
                    }
                }
                yield return new PostAggreagate<Tuple<int, int, int, int, int, int>>(postPair.Key)
                {
                    Data =
                        new Tuple<int, int, int, int, int, int>(posCount, posLikes, posShares,
                            comCount,
                            comLikes,
                            comShares),
                };
            }
        }

        public static IEnumerable<PostAggreagate<double>> TopPostsByCommentsMark(this IEnumerable<Post> posts, int count)
        {
            var grouped = posts.Where(x => !string.IsNullOrEmpty(x.PostParentOrgId)).Aggreage(post =>
              new PostAggreagate<double>(post) { PostOrgid = post.PostParentOrgId });
            return grouped.Select(element => new PostAggreagate<double>(element.Key)
            {
                Data = element.Value.Sum(x=>x.MarkValue),
            });
        }
        

        public static IEnumerable<PostAggreagate<int>> TopPostsByCommentsCount(this IEnumerable<Post> posts, int count)
        {
            var grouped = posts.Where(x=>!string.IsNullOrEmpty(x.PostParentOrgId)).Aggreage(post =>
               new PostAggreagate<int>(post){PostOrgid = post.PostParentOrgId});
            return grouped.Select(element => new PostAggreagate<int>(element.Key)
            {
                Data = element.Value.Count,
            });
        }

        public static IEnumerable<PostAggreagate<Tuple<string, int, int>>> TopWordsInTime(this IEnumerable<Post> posts,HashSet<string> stopWords)
        {
            var grouped = posts.Aggreage(post =>
                new PostAggreagate<Tuple<string, int, int>>(post));
            return grouped.Select(group => WordCloudAggegator(group.Value, 1, stopWords).Single());
        }

        public static IEnumerable<PostAggreagate<Tuple<string, int, int>>> TrackedWordsInTime(
            this IEnumerable<Post> posts, IEnumerable<string> trackedWords)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<Tuple<string, int, int>>(post));
            var tmpDict = trackedWords.ToDictionaryDistinct(x => x, x => 0);
            foreach (var group in grouped)
            {
                var words = group.Value.SelectMany(x => x.NormalizeMessage.SplitByWhitespaces());
                foreach (var word in words)
                {
                    if (tmpDict.ContainsKey(word))
                        tmpDict[word] += 1;
                }
                foreach (var word in tmpDict)
                {
                    yield return new PostAggreagate<Tuple<string, int, int>>(group.Key)
                    {
                        Data = new Tuple<string, int, int>(word.Key, word.Value, word.Value),
                    };
                }
            }
        }

        public static IEnumerable<PostAggreagate<int>> TopPostsByLikes(this IEnumerable<Post> posts)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<int>(post));
            return grouped.SelectMany(element => element.Value.OrderBy(x => x.Strong),
                (element, post) => new PostAggreagate<int>(element.Key)
                {
                    Data = post.Strong,
                    Date = post.MessageDateCreate,
                });
        }

        public static IEnumerable<PostAggreagate<Tuple<int, int, string>>> TopAuthorsByPosLikes(
            this IEnumerable<Post> posts, int count)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<Tuple<int, int, string>>(post)
            {
                AuthorOrgid = post.PostAuthorOrgId
            });
            return grouped.OrderByDescending(x => x.Value.Count).Take(count).Select(element =>
                new PostAggreagate<Tuple<int, int, string>>(element.Key)
            {
                Data = Tuple.Create(element.Value.Count,element.Value.Sum(x=>x.Strong),element.Key.AuthorOrgid),
            });
        }

        public static IEnumerable<PostAggreagate<Tuple<int, string>>> TopAuthorsByPosCount(this IEnumerable<Post> posts,
            int count)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<Tuple< int, string>>(post)
            {
                AuthorOrgid = post.PostAuthorOrgId
            });
            return grouped.OrderBy(x=>x.Value.Count).Select(element => 
                new PostAggreagate<Tuple< int, string>>(element.Key)
            {
                Data = Tuple.Create(element.Value.Count, element.Key.AuthorOrgid),
            });
        }

        public static IEnumerable<PostAggreagate<Tuple<int, int,int,double>>> SummaryPosts(this IEnumerable<Post> posts)
        {
            var grouped = posts.Aggreage(post => new PostAggreagate<Tuple<int, int,int,double>>(post));
            return grouped.OrderBy(x => x.Value.Count).Select(element =>
                new PostAggreagate<Tuple<int, int,int,double>>(element.Key)
            {
                Data = Tuple.Create(element.Value.Count, element.Value.Sum(x => x.Strong), 
                    element.Value.Sum(x => x.Shares), element.Value.Sum(x => x.MarkValue)),
            });
        }
    }
}