using Semanticer.TextAnalyzer;
using Semanticer.Common.Enums;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Semanticer
{
	public class SemanticProccessor
	{
		private readonly IPostSematicEvaluator evaluator;
		public SemanticProccessor()
		{
			evaluator = new TrainablePostSematicEvaluator (LearnigAlghoritm.NaiveBayes, "en-US");
		}

		public SemanticResult Process (string toEvaluate)
		{
			var evaluated = evaluator.Evaluate (toEvaluate, "en-US");
			var mark = SelectBestMark (evaluated);
			mark.Text = toEvaluate;
			return mark;
		}

		static SemanticResult SelectBestMark (IDictionary<PostMarkType, double> evaluated)
		{
			double positiveMark = evaluated [PostMarkType.Positive];
			double negativeMark = evaluated [PostMarkType.Negative];
			double neutralMark = evaluated [PostMarkType.Neutral];
			if (negativeMark < 0.55 && positiveMark < 0.55) 
			{
				if (neutralMark < 0.55) 
				{
					return new SemanticResult 
					{
						Propability = 1 - 0.55,
						Result = PostMarkType.NonCaluculated,
					};
				} 
				return new SemanticResult 
				{
					Propability = neutralMark,
					Result = PostMarkType.Neutral,
				};
			}
			if (negativeMark > positiveMark) 
			{
				return new SemanticResult 
				{
					Propability = negativeMark,
					Result = PostMarkType.Negative,
				};
			}
			return new SemanticResult 
			{
				Propability = positiveMark,
				Result = PostMarkType.Positive,
			};
		}
	}
}
