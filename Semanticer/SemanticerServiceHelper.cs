using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semanticer
{
    public class SemanticerServiceHelper
    {
        private readonly Lazy<ISemanticProccessor> trainedLazy;
        public ISemanticProccessor Proccessor => trainedLazy.Value;

        public SemanticerServiceHelper(ISemanticProccessor processor)
        {
            trainedLazy = new Lazy<ISemanticProccessor>(()=>WaitForTrain(processor));
        }

        private ISemanticProccessor WaitForTrain(ISemanticProccessor processor)
        {
            while (!processor.IsTrained())
            {
                Task.Delay(1000).Wait();
            }
            return processor;
        }
    }
}
