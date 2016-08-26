using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using Semanticer.Common;

namespace Semanticer.Wcf
{
    internal class SemanticerService : ServiceBase
    {
        private ServiceHost selfHostedService;

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
            selfHostedService?.Close();
            selfHostedService = null;
        }

        public void ServiceStart()
        {
            selfHostedService?.Close();
            selfHostedService = StartServiceHost(typeof(SemanticProccessor), typeof(ISemanticProccessor));
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