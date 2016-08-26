using System.ServiceModel;

namespace Semanticer.Streamer
{
    [ServiceContract]
    public interface ITweeterStreamDownloader
    {
        [OperationContract]
        TweetWithSemantic[] Tweets();

        [OperationContract]
        DailyStats DailyStats();

        [OperationContract]
        void Start();
    }
}