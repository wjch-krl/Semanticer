using System.Collections.Generic;
using Semanticer.Common.Enums;

namespace Semanticer.TextAnalyzer
{
    public interface IPostSematicEvaluator
    {
		IDictionary<PostMarkType, double> Evaluate (string msg, string lang = null);
	}
}