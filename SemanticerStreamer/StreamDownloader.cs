using System;
using System.Threading;
using System.Threading.Tasks;
using Semanticer.Common;
using Tweetinvi.Core.Streaming;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Tweetinvi.Streams;
using Tweetinvi;
using Tweetinvi.Events;

namespace Semanticer.Streamer
{
    public class StreamDownloader
    {
        private readonly TweetWithSemantic[] tweets;
        private Counter idx;
        private readonly DailyStats stats;

        public StreamDownloader(int maxOldTweets)
        {
            this.tweets = new TweetWithSemantic[maxOldTweets];
            this.idx = new Counter(maxOldTweets);
            this.stats = new DailyStats();
            StartStream();
        }

        public TweetWithSemantic[] Tweets => tweets;

        public DailyStats DailyStats => stats;

        private void StartStream()
        {
            var stream = PrepareStream();
            stream.StartStreamAsync();
        }

        private ISampleStream PrepareStream()
        {
            var userTimelineStream = Stream.CreateSampleStream();
            userTimelineStream.TweetReceived += UserTimelineStreamOnTweetReceived;
            return userTimelineStream;
        }

        private void UserTimelineStreamOnTweetReceived(object sender, TweetReceivedEventArgs tweetReceivedEventArgs)
        {
            var tweet = tweetReceivedEventArgs.Tweet;
            if (tweet.Language == Language.English)
            {
                AddTweet(tweet);
            }
        }

        private void AddTweet(ITweet tweet)
        {
//            var semantics = Sempro.Process(tweet.Text);
//            tweets[idx++.Value] = new TweetWithSemantic
//            {
//                Semantics = semantics,
//                Tweet = tweet,
//            };
//            stats.Add(DateTime.UtcNow, semantics);
        }
    }
}

