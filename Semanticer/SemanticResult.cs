using System;
using Semanticer.Common.Enums;
namespace Semanticer
{
    [Serializable]
    public class SemanticResult
    {
		public PostMarkType Result { get; set; }
        public double Propability { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Text} is {Result} (With propability: {Propability})";
        }
    }
}