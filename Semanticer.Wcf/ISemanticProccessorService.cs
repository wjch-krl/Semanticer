using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Semanticer.Wcf
{
    [ServiceContract]
    public interface ISemanticProccessorService
    {
        [OperationContract]
        SemanticResult Process(string toEvaluate);
    }
}
