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
            serviceHosts.Add(StartServiceHost(typeof(SemanticProccessor), typeof(ISemanticProccessor)));
            serviceHosts.Add(StartServiceHost(typeof(TweeterStreamDownloader), typeof(ITweeterStreamDownloader)));
            serviceHosts.Add(StartServiceHost(typeof(ControlService), typeof(IControlService)));
        }


        private ServiceHost StartServiceHost(Type implementationType, Type contracType)
        {
            var service = new ServiceHost(implementationType, new Uri(Constans.ServiceBaseUrl));
            service.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            service.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
            service.AddServiceEndpoint(contracType, new NetNamedPipeBinding(), contracType.Name);
            service.Open();
            return service;
        }
    }
}