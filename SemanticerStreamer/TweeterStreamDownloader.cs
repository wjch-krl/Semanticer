using System;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Utils;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Tweetinvi;
using Tweetinvi.Events;
using Semanticer;

namespace Semanticer.Streamer
{
    public class TweeterStreamDownloader : ITweeterStreamDownloader
    {
        private readonly TweetWithSemantic[] tweets;
        private Counter idx;
        private readonly DailyStats stats;
        private bool running;
        private readonly ISemanticProccessor service;

        public TweeterStreamDownloader()
        {
            this.tweets = new TweetWithSemantic[Constans.MaxOldTweets];
            this.idx = new Counter(Constans.MaxOldTweets);
            this.stats = new DailyStats();
            //service = new SemanticProccessor();
        }

        public TweetWithSemantic[] Tweets() => tweets;

        public DailyStats DailyStat() => stats;

        public void Start()
        {
            Validate();
            var stream = PrepareStream();
            stream.StartStreamAsync();
        }

        private void Validate()
        {
            lock (service)
            {
                if (running)
                {
                    throw new InvalidOperationException("Already running");
                }
                running = true;
            }
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
            var semantics = service.Process(tweet.Text);
            tweets[idx++.Value] = new TweetWithSemantic
            {
                Semantics = semantics,
                Tweet = tweet,
            };
            stats.Add(DateTime.UtcNow, semantics);
        }
    }
}

