using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using Semanticer.Common;
using Semanticer.Streamer;

namespace Semanticer.Wcf
{
    internal class SemanticerService : ServiceBase
    {
        private readonly List<ServiceHost> serviceHosts;

        public SemanticerService()
        {
            serviceHosts = new List<ServiceHost>();
        }

        protected override void OnStart(string[] args)
        {
            ServiceStart();
        }

        protected override void OnStop()
        {
            ServiceStop();
        }

        private void ServiceStop()
        {
            foreach (var service in serviceHosts)
            {
                service.Close();
            }
            serviceHosts.Clear();
        }

        public void ServiceStart()
        {
            ServiceStop();
            var startHost = GetHostCreator();
            serviceHosts.Add(startHost(typeof(SemanticProccessor), typeof(ISemanticProccessor)));
            serviceHosts.Add(startHost(typeof(TweeterStreamDownloader), typeof(ITweeterStreamDownloader)));
            serviceHosts.Add(startHost(typeof(ControlService), typeof(IControlService)));
        }

        private Func<Type, Type, ServiceHost> GetHostCreator()
        {
            if (Constans.UsePipe)
            {
                return StartPipeServiceHost;
            }
            return StartHttpServiceHost;
        }


        private ServiceHost StartHttpServiceHost(Type implementationType, Type contracType)
        {
            var service = new ServiceHost(implementationType, new Uri($"{Constans.ServiceBaseHttpUrl}/{contracType.Name}"));
            service.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            service.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            service.AddServiceEndpoint(contracType, new BasicHttpBinding(), contracType.Name);
            service.Open();
            return service;
        }

        private ServiceHost StartPipeServiceHost(Type implementationType, Type contracType)
        {
            var service = new ServiceHost(implementationType, new Uri(Constans.ServiceBasePipeUrl));
            service.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            service.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            service.AddServiceEndpoint(contracType, new NetNamedPipeBinding(), contracType.Name);
            service.Open();
            return service;
        }
    }
}