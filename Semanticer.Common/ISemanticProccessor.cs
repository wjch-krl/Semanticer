using System.ServiceModel;

namespace Semanticer.Common
{
    [ServiceContract]
    public interface ISemanticProccessor
    {
        [OperationContract]
        SemanticResult Process(string toEvaluate);

        [OperationContract]
        bool IsTrained();
    }
}
