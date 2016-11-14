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
            //return IsServiceAviable ? GetTrainedSemanticProccessorCore() : new SemanticProccessor();
            return GetTrainedSemanticProccessorCore();
        }

        private static ISemanticProccessor GetTrainedSemanticProccessorCore()
        {
            var processor = GetService<ISemanticProccessor>();
            return processor;
        }

        public static ITweeterStreamDownloader GetStartedTweeterStreamDownloader()
        {
           // var stream = IsServiceAviable ? GetTweeterStreamDownloaderCore() : new TweeterStreamDownloader();
            var stream = GetTweeterStreamDownloaderCore();
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
            var factory = Constans.UsePipe ? CreatePipeChannelFactory<T>() : CreateHtttpChannelFactory<T>();
            return factory.CreateChannel();
        }

        private static ChannelFactory<T> CreatePipeChannelFactory<T>( )
        {
            var type = typeof(T);
            Uri address = new Uri($"{Constans.ServiceBasePipeUrl}/{type.Name}/");
            ChannelFactory<T> pipeFactory =
                new ChannelFactory<T>(new NetNamedPipeBinding() {MaxReceivedMessageSize = int.MaxValue},
                    new EndpointAddress(address));
            return pipeFactory;
        }

        private static ChannelFactory<T> CreateHtttpChannelFactory<T>()
        {
            var type = typeof(T);
            Uri address = new Uri($"{Constans.ServiceBaseHttpUrl}/{type.Name}/{type.Name}/");
            ChannelFactory<T> pipeFactory =
                new ChannelFactory<T>(new NetHttpBinding() {MaxReceivedMessageSize = int.MaxValue},
                    new EndpointAddress(address));
            return pipeFactory;
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
