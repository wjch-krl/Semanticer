using System;
using Semanticer.Common;

namespace Semanticer.Wcf
{
    public class SemanticProccessorService : ISemanticProccessor
    {
        private static readonly Lazy<SemanticProccessor> ProcessorLazy;

        static SemanticProccessorService()
        {
            ProcessorLazy = new Lazy<SemanticProccessor>(() => new SemanticProccessor());
        }

        private static SemanticProccessor Proccessor => ProcessorLazy.Value;
        public SemanticResult Process(string toEvaluate)
        {
            return Proccessor.Process(toEvaluate);
        }

        public bool IsTrained() => Proccessor.IsTrained();
    }
}
