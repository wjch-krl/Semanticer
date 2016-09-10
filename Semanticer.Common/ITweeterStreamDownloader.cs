using System.ServiceModel;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    [ServiceContract]
    public interface ITweeterStreamDownloader
    {
        [OperationContract]
        TweetWithSemantic[] Tweets();

        [OperationContract]
        DailyStats DailyStat();

        [OperationContract]
        void Start();


    }
}