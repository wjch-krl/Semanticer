using Semanticer.Classifier.Common;
using SharpEntropy;
using SharpEntropy.IO;

namespace Semanticer.Classifier.Textual.MaxEnt
{
    internal class PivotGisTrainer : GisTrainer, IGisModelReader
    {
        private readonly IPivotWordProvider pivotWordProvider;

        public PivotGisTrainer(IPivotWordProvider pivotWordProvider, bool useSlackParameter = false,
            double smoothingObservation = 0.1)
            : base(useSlackParameter, smoothingObservation)
        {
            this.pivotWordProvider = pivotWordProvider;
        }

        private double multiper = 1.0;
        private string tmpWord;
        public new void GetPredicateData(string predicateLabel, int[] featureCounts, double[] outcomeSums)
        {
            bool isBigram = predicateLabel.Contains(" ");
            if (isBigram)
            {
                GetMultigramPredicateData(predicateLabel, featureCounts, outcomeSums);
                return;
            }
            GetPivotPredicateData(predicateLabel, featureCounts, outcomeSums);
        }


        private void GetPivotPredicateData(string predicateLabel, int[] featureCounts, double[] outcomeSums)
        {
            //Pobranie mnożnika obecnego słowa
            double newMultiper = pivotWordProvider.Multiper(predicateLabel);
            //Jeśli dana cecha nie była obecna w zbiorze treningowym - powrót
            if (!GetPredicates().ContainsKey(predicateLabel))
            {
                //ustawienie mnożnika modyfikującego następne słowo
                multiper = newMultiper;
                return;
            }
            PatternedPredicate predicate = GetPredicates()[predicateLabel];
            if (predicate != null)
            {
                int[] activeOutcomes = GetOutcomePatterns()[predicate.OutcomePattern];
                //Obliczenie sum prawdopodobieństw dla danej cechy
                for (int currentActiveOutcome = 1; currentActiveOutcome < activeOutcomes.Length; currentActiveOutcome++)
                {
                    int outcomeIndex = activeOutcomes[currentActiveOutcome];
                    featureCounts[outcomeIndex]++;
                    outcomeSums[outcomeIndex] +=/* multiper **/ predicate.GetParameter(currentActiveOutcome - 1);
                }
            }
            tmpWord = predicateLabel;
            multiper = newMultiper;
        }

        private void GetMultigramPredicateData(string predicateLabel, int[] featureCounts, double[] outcomeSums)
        {
            if (!GetPredicates().ContainsKey(predicateLabel))
            {
                return;
            }
            PatternedPredicate predicate = GetPredicates()[predicateLabel];
            if (predicate != null)
            {
                int[] activeOutcomes = GetOutcomePatterns()[predicate.OutcomePattern];

                for (int currentActiveOutcome = 1; currentActiveOutcome < activeOutcomes.Length; currentActiveOutcome++)
                {
                    int outcomeIndex = activeOutcomes[currentActiveOutcome];
                    featureCounts[outcomeIndex]++;
                    outcomeSums[outcomeIndex] += predicate.GetParameter(currentActiveOutcome - 1);
                }
            }
        }
    }
}
