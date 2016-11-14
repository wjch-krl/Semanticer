using System;
using Semanticer.TextAnalyzer;
using Semanticer.Common.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Semanticer.Common;

namespace Semanticer
{
    public class SemanticProccessor : ISemanticProccessor
    {
        private static readonly Lazy<ISematicEvaluator> _bestEvaluator;
        static SemanticProccessor()
        {
            _bestEvaluator = new Lazy<ISematicEvaluator>(CreateEvaluator);
        }

        private static TrainableSematicEvaluator CreateEvaluator()
        { 
            var bowFactory = new BagOfWordsTransformerFactory();
            var bestEvaluator = new TrainableSematicEvaluator(LearnigAlghoritm.NaiveBayes, "en-US", bowFactory);
			Console.WriteLine("Trained");
            return bestEvaluator;
        }

        public SemanticResult Process(string toEvaluate)
        {
            var evaluated = _bestEvaluator.Value.Evaluate(toEvaluate);
            var mark = SelectBestMark(evaluated);
            mark.Text = toEvaluate;
            return mark;
        }

        public static SemanticResult SelectBestMark(IDictionary<MarkType, double> evaluated)
        {
            double positiveMark = GetMarkPropability(evaluated, MarkType.Positive);
            double negativeMark = GetMarkPropability(evaluated,MarkType.Negative);
            double neutralMark = GetMarkPropability(evaluated,MarkType.Neutral);
            if (negativeMark < 0.55 && positiveMark < 0.55)
            {
                if (neutralMark < 0.55)
                {
                    return new SemanticResult
                    {
                        Propability = 1 - 0.55,
                        Result = MarkType.NonCaluculated,
                    };
                }
                return new SemanticResult
                {
                    Propability = neutralMark,
                    Result = MarkType.Neutral,
                };
            }
            if (negativeMark > positiveMark)
            {
                return new SemanticResult
                {
                    Propability = negativeMark,
                    Result = MarkType.Negative,
                };
            }
            return new SemanticResult
            {
                Propability = positiveMark,
                Result = MarkType.Positive,
            };
        }

        private static double GetMarkPropability(IDictionary<MarkType, double> evaluated, MarkType mark)
        {
            return evaluated.ContainsKey(mark) ? evaluated[mark] : 0.0;
        }

        public bool IsTrained() => _bestEvaluator.IsValueCreated;
    }
}
