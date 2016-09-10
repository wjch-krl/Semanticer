using System.ServiceModel;

namespace Semanticer.Common
{
    [ServiceContract]
    public interface IControlService
    {
        [OperationContract]
        bool IsAlive();
    }
}
