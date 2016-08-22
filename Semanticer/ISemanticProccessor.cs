using System.ServiceModel;

namespace Semanticer
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
