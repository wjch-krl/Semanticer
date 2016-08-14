using Semanticer.TextAnalyzer;

namespace Semanticer
{
    public static class SemanticProccessor
    {
        private static IPostSematicEvaluator evaluator;
        static SemanticProccessor()
        {
            evaluator = new TrainablePostSematicEvaluator();
        }

        public static SemanticResult Process(string toEvaluate)
        {
            var evaluated = evaluator.Evaluate("ën-US", toEvaluate);
            return new SemanticResult
            {
                Text = toEvaluate,
                Propability = 0.23,
                Result = SemanticType.Positive,
            };
        }
    }
}
