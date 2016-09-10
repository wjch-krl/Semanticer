using System.Collections.Generic;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public interface ISematicEvaluator
    {
		IDictionary<MarkType, double> Evaluate (string msg);
	}
}