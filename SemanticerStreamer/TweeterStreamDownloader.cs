using System;
using System.Threading.Tasks;
using Semanticer.Common;
using Semanticer.Common.DataModel;
using Semanticer.Common.Utils;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Tweetinvi;
using Tweetinvi.Events;
using Semanticer;
using Semanticer.Common.Enums;

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
            service = new SemanticProccessor();
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
            lock (idx)
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
            var stream = Stream.CreateSampleStream(Credentials());
            stream.TweetReceived += StreamOnTweetReceived;
            return stream;
        }

        private static TwitterCredentials Credentials()
        {
            return new TwitterCredentials(
                "zEBnXsdy8jqN8n6BuWi7h70ay",
                "aH87R2oW65QRtcdjdVquJPcwRPb7Rgd44azuaIRFFPcPscAhqu",
                "388464418-Mb5lSMPHsmLX7ykljkJIf7Z1ouxbXOC2TsY75B1P",
                "dxl5IeTMahrknEOGRWsA6IeexVfCpxbO2OZ8LBgnN6MDP");
        }

        private void StreamOnTweetReceived(object sender, TweetReceivedEventArgs tweetReceivedEventArgs)
        {
            var tweet = tweetReceivedEventArgs.Tweet;
            if (tweet.Language == Language.English)
            {
                AddTweet(tweet);
            }
            else
            {
                AddTweetWithUnknownLanguage(tweet);
            }
        }

        private void AddTweetWithUnknownLanguage(ITweet tweet)
        {
            var semantics = new SemanticResult
            {
                Propability = 1,
                Result = MarkType.NonSupportedLanguage,
                Text = tweet.Text,
            };
            AddTweetCore(tweet,semantics);
        }

        //Random ran = new Random();
        private void AddTweet(ITweet tweet)
        {
            var semantics = 
//                new SemanticResult
//            {
//                Propability = ran.NextDouble(),
//                Result = (MarkType) ran.Next(3) + 1,
//                Text = tweet.Text,
//            }; 
            service.Process(tweet.Text);
            AddTweetCore(tweet, semantics);
        }

        private void AddTweetCore(ITweet tweet, SemanticResult semantics)
        {
            tweets[idx.Value] = new TweetWithSemantic(tweet,semantics);
            idx++;
            stats.Add(DateTime.UtcNow, semantics);
        }
    }
}

