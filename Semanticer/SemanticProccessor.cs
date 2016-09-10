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
        private IPostSematicEvaluator bestEvaluator;
        private IList<TrainablePostSematicEvaluator> evaluators;
        public IList<TrainablePostSematicEvaluator> Evaluators => evaluators;

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
                new TrainablePostSematicEvaluator(LearnigAlghoritm.Knn, "en-US", d2vFactory),
                new TrainablePostSematicEvaluator(LearnigAlghoritm.Knn, "en-US", bowFactory),
         //       new TrainablePostSematicEvaluator(LearnigAlghoritm.Svm, "en-US", bowFactory),
                new TrainablePostSematicEvaluator(LearnigAlghoritm.Svm, "en-US", d2vFactory),
                new TrainablePostSematicEvaluator(LearnigAlghoritm.NaiveBayes, "en-US", bowFactory),
                new TrainablePostSematicEvaluator(LearnigAlghoritm.MaxEnt, "en-US", bowFactory)
            };
            bestEvaluator = evaluators.First();
            isTrained = true;
        }

        public SemanticResult Process(string toEvaluate)
        {
            return Process(toEvaluate, bestEvaluator);
        }

        public SemanticResult Process(string toEvaluate, IPostSematicEvaluator evaluator)
        {
            var evaluated = evaluator.Evaluate(toEvaluate, "en-US");
            var mark = SelectBestMark(evaluated);
            mark.Text = toEvaluate;
            return mark;
        }

        public static SemanticResult SelectBestMark(IDictionary<PostMarkType, double> evaluated)
        {
            double positiveMark = GetMarkPropability(evaluated, PostMarkType.Positive);
            double negativeMark = GetMarkPropability(evaluated,PostMarkType.Negative);
            double neutralMark = GetMarkPropability(evaluated,PostMarkType.Neutral);
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

        private static double GetMarkPropability(IDictionary<PostMarkType, double> evaluated, PostMarkType mark)
        {
            return evaluated.ContainsKey(mark) ? evaluated[mark] : 0.0;
        }

        public bool IsTrained()
        {
            return isTrained;
        }
    }
}
