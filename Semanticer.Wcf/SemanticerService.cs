using System.ServiceModel;
using System.ServiceProcess;

namespace Semanticer.Wcf
{
    internal class SemanticerService : ServiceBase
    {
        private ServiceHost selfHostedService;

        protected override void OnStart(string[] args)
        {
            ServiceStart(args);
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

        public void ServiceStart(string[] args)
        {
            selfHostedService?.Close();
            selfHostedService = new ServiceHost(typeof(SemanticProccessor));
            selfHostedService.Open();
        }
    }
}