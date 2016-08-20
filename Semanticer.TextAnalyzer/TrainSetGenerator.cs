using System.Collections.Generic;
using System.Linq;
using Semanticer.Classifier.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public class TrainSetGenerator
    {
        private readonly string langCode;

        public TrainSetGenerator(string langCode)
        {
            this.langCode = langCode;
            PositiveSeeds = new List<string>();
            NegativeSeeds = new List<string>();
            NeutralSeeds = new List<string>();
        }

        public IList<string> PositiveSeeds { get; }
        public IList<string> NegativeSeeds { get; }
        public IList<string> NeutralSeeds { get; }
        public IPostDataProvider SourceDataProvider { get; set; }
        public ITextAnalizerDataProvider DestinationDataProvider { get; set; }

        public void ExtractData()
        {
            var positive = ExtractTrainData(PositiveSeeds.ToArray(), NegativeSeeds.ToArray());
            var negative = ExtractTrainData(NegativeSeeds.ToArray(), PositiveSeeds.ToArray());
            var neutral = ExtractTrainData(NeutralSeeds.ToArray(), PositiveSeeds.Concat(NeutralSeeds).ToArray());
            var postsToSave = PrepereToSave(positive, negative, neutral);
            DestinationDataProvider.SavePostsAsTrainingData(postsToSave);
        }

        private IEnumerable<Post> ExtractTrainData(string[] seeds,
            string[] antiSeeds)
        {
            var posts = SearchForSeeds(seeds);
            posts = ProcessPmi(posts, seeds, antiSeeds);
            return posts;
        }

        private IEnumerable<Post> ProcessPmi(IEnumerable<Post> posts, string[] seeds, string[] antiSeeds)
        {
            foreach (var post in posts)
            {
                if (IsMatch(post, seeds, antiSeeds))
                {
                    yield return post;
                }
                else
                {
                    yield break;
                }
            }
        }

        private bool IsMatch(Post post, string[] seeds, string[] antiSeeds)
        {
            var wordCount = 0;
            var seedOccurances = 0;
            var antiSeedOccurances = 0;
            foreach (var word in post.NormalizeMessage)
            {
                wordCount++;
                if (seeds.Contains(word))
                {
                    seedOccurances++;
                }
                if (antiSeeds.Contains(word))
                {
                    antiSeedOccurances++;
                }
            }
//            double seedPropability = (double)seedOccurances / wordCount;
//            double antiSeedPropability = (double)antiSeedOccurances / wordCount;
//            double combinedPropability = (double) (seedOccurances + antiSeedOccurances)/wordCount;
//            double pmi = Math.Log(combinedPropability/(seedPropability*antiSeedPropability));
            return wordCount != 0 && seedOccurances > 0 && antiSeedOccurances == 0;
        }

        private IEnumerable<Post> SearchForSeeds(string[] seeds)
        {
            return SourceDataProvider.FullTextSearchPosts(seeds.ToArray()).Where(x => x.Lang == langCode);
        }

        private IList<Post> PrepereToSave(IEnumerable<Post> positive, IEnumerable<Post> negative,
            IEnumerable<Post> neutral)
        {
            var postsList = new List<Post>();
            postsList.AddRange(UpdateSentiment(positive, PostMarkType.Positive));
            postsList.AddRange(UpdateSentiment(negative, PostMarkType.Negative));
            postsList.AddRange(UpdateSentiment(neutral, PostMarkType.Neutral));
            return postsList;
        }

        private IEnumerable<Post> UpdateSentiment(IEnumerable<Post> posts, PostMarkType postMarkType)
        {
            return posts.Select(x =>
            {
                x.ChangeMark(postMarkType);
                return x;
            });
        }
    }
}