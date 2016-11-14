using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Semanticer;
using Semanticer.Classifier.Common;
using Semanticer.Classifier.Numeric.Svm;
using Semanticer.Common;
using Semanticer.Common.Enums;
using Semanticer.TextAnalyzer;
using SharpEntropy;

namespace Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            var corpa = new TnCorpaCreator(new NgramTokenizerNormalizerFactory("en-US",1).Create(), ClassifierConstants.TnTwitterTrainDatasetPath,ClassifierConstants.TnTwitterTestDatasetPath);
            corpa.CreateCorpa(@"C:\mgr\Semanticer\aclImdb\tweeter_corpa.txt");

            var d2vFactory = new Doc2VecTransformerFactory();
            d2vFactory.CreateTextTransformer();
            var bowFactory = new BagOfWordsTransformerFactory();
            var evaluators = new[]
            {
                new TrainableSematicEvaluator(LearnigAlghoritm.Knn, "en-US", d2vFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.Knn, "en-US", bowFactory),
         //       new TrainableSematicEvaluator(LearnigAlghoritm.Svm, "en-US", bowFactory),
          //      new TrainableSematicEvaluator(LearnigAlghoritm.Svm, "en-US", d2vFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.NaiveBayes, "en-US", bowFactory),
                new TrainableSematicEvaluator(LearnigAlghoritm.MaxEnt, "en-US", bowFactory)
            };
            ITrainingEventReader reader = new TkinkNookTweeterEventReader(ClassifierConstants.TnTwitterTrainDatasetPath,
                new SimpleTokenizer());

            //(svm.classifier as Svm).CerateTestProblem(reader);
            List<MarkType>[] markTypes = new List<MarkType>[evaluators.Length +1];
            for (int i = 0; i < markTypes.Length; i++)
            {
                markTypes[i] = new List<MarkType>();
            }
            int idx = 0;
            do
            {
                var testMsg = reader.ReadNextEvent();
                markTypes[0].Add(testMsg.GetMarkType());
                for (int i = 0; i < evaluators.Length; i++)
                {
                    markTypes[i+1].Add(SemanticProccessor.SelectBestMark(evaluators[i].Evaluate(testMsg.GetContext()[0])).Result);
                }

            } while (reader.HasNext() && idx++ < 7000);
            using (var output = new StreamWriter("result_twitter"))
            {
                for (int i = 1; i < markTypes.Length; i++)
                {
                    var matrix = new ClassifiationMatrix(markTypes[0].Take(idx).ToArray(), markTypes[i]);
                    {
                        output.WriteLine(matrix);
                    }
                }
            }
        }

        private static void EvaluateFromConsole(ISemanticProccessor processor)
        {
            try
            {
                Console.WriteLine("Enter text:");
                var input = Console.ReadLine();
                Console.WriteLine(processor.Process(input));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    internal class SimpleTokenizer : ITokenizer
    {
        public string[] Tokenize(string input)
        {
            return new[] {input};
        }
    }
}
