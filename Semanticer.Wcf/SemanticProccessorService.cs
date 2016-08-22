using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Semanticer.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SemanticProccessorService" in both code and config file together.
    public class SemanticProccessorService : ISemanticProccessor
    {
        private static readonly Lazy<SemanticProccessor> ProcessorLazy;

        static SemanticProccessorService()
        {
            ProcessorLazy = new Lazy<SemanticProccessor>(() => new SemanticProccessor());
        }

        internal static SemanticProccessor Proccessor => ProcessorLazy.Value;
        public SemanticResult Process(string toEvaluate)
        {
            return Proccessor.Process(toEvaluate);
        }

        public bool IsTrained() => Proccessor.IsTrained();
    }
}
