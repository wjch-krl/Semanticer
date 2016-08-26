using System;
using System.ServiceModel;
using Semanticer.Common;

namespace Semanticer.WcfClient
{
    public static class ServiceResolver
    {
        private static ISemanticProccessor GetSemanticProcessor()
        {
            return GetService<ISemanticProccessor>();
        }

        public static ISemanticProccessor GetTrainedSemanticProccessor()
        {
            var processor = GetSemanticProcessor();
            var helper = new SemanticerServiceHelper(processor);
            return helper.Proccessor;
        }

        private static T GetService<T>()
        {
            var type = typeof(T);
            Uri baseAddress = new Uri($"{Constans.ServiceBaseUrl}/{type.Name}/");
            ChannelFactory<T> pipeFactory = new ChannelFactory<T>(new NetNamedPipeBinding() { MaxReceivedMessageSize = int.MaxValue }, new EndpointAddress(baseAddress));
            return pipeFactory.CreateChannel();
        }
    }
}
