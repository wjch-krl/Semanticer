using System;
using Semanticer.Common.Enums;

namespace Semanticer.Common
{
    [Serializable]
    public class SemanticResult
    {
		public MarkType Result { get; set; }
        public double Propability { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Text} is {Result} (With propability: {Propability})";
        }
    }
}