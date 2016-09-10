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
        private volatile bool isTrained;
        private ISematicEvaluator bestEvaluator;
        private IList<TrainableSematicEvaluator> evaluators;
        public IList<TrainableSematicEvaluator> Evaluators => evaluators;

        public SemanticProccessor()
        {
            isTrained = false;
            Task.Factory.StartNew(CreateEvaluator);
        }

        private void CreateEvaluator()
        {
            var d2vFactory = new Doc2VecTransformerFactory();
            var bowFactory = new BagOfWordsTransformerFactory();
            evaluators = new[]
            {
                new TrainableSematicEvaluator(LearnigAlghoritm.Knn, "en-US", d2vFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.Knn, "en-US", bowFactory),
         //       new TrainableSematicEvaluator(LearnigAlghoritm.Svm, "en-US", bowFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.Svm, "en-US", d2vFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.NaiveBayes, "en-US", bowFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.MaxEnt, "en-US", bowFactory)
            };
            bestEvaluator = evaluators.First();
            isTrained = true;
			Console.WriteLine("Trained");
        }

        public SemanticResult Process(string toEvaluate)
        {
            return Process(toEvaluate, bestEvaluator);
        }

        public SemanticResult Process(string toEvaluate, ISematicEvaluator evaluator)
        {
            var evaluated = evaluator.Evaluate(toEvaluate);
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

        public bool IsTrained()
        {
            return isTrained;
        }
    }
}
