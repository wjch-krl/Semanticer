using System;
using System.ServiceModel;
using Semanticer.Common;

namespace Semanticer.WcfClient
{
    public static class ServiceResolver
    {
        private static readonly Lazy<bool> _isAviable;

        static ServiceResolver()
        {
            _isAviable = new Lazy<bool>(CheckServiceSatatus);
        }

        public static ISemanticProccessor GetTrainedSemanticProccessor()
        {
            return IsServiceAviable ? GetTrainedSemanticProccessorCore() : null; //new SemanticProccessor();
        }

        private static ISemanticProccessor GetTrainedSemanticProccessorCore()
        {
            var processor = GetService<ISemanticProccessor>();
            var helper = new SemanticerServiceHelper(processor);
            return helper.Proccessor;
        }

        public static ITweeterStreamDownloader GetStartedTweeterStreamDownloader()
        {
            var stream = IsServiceAviable ? GetTweeterStreamDownloaderCore() : new TweeterStreamDownloaderStub();
            stream.Start();
            return stream;
        }

        private static ITweeterStreamDownloader GetTweeterStreamDownloaderCore()
        {
            var processor = GetService<ITweeterStreamDownloader>();
            return processor;
        }

        public static bool IsServiceAviable => _isAviable.Value;

        private static T GetService<T>()
        {
            var type = typeof(T);
            Uri address = new Uri($"{Constans.ServiceBaseUrl}/{type.Name}/");
            ChannelFactory<T> pipeFactory = new ChannelFactory<T>(new NetNamedPipeBinding() { MaxReceivedMessageSize = int.MaxValue }, new EndpointAddress(address));
            return pipeFactory.CreateChannel();
        }

        private static bool CheckServiceSatatus()
        {
            try
            {
                var service = GetService<IControlService>();
                return service.IsAlive();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
