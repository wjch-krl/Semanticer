using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Semanticer;
using Semanticer.Classifier.Common;
using Semanticer.Common;
using Semanticer.Common.Enums;
using Semanticer.TextAnalyzer;

namespace Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            SemanticProccessor processor = new SemanticProccessor();
            while (!processor.IsTrained())
            {
                Task.Delay(1000).Wait();
            }
            var reader = new ImdbFileEventReader(ClassifierConstants.ImdbTestDatasetPath,
                new SimpleTokenizer());
            var evaluators = processor.Evaluators;
            List<PostMarkType>[] markTypes = new List<PostMarkType>[evaluators.Count +1];
            for (int i = 0; i < markTypes.Length; i++)
            {
                markTypes[i] = new List<PostMarkType>();
            }

            do
            {
                var testMsg = reader.ReadNextEvent();
                markTypes[0].Add(testMsg.GetMarkType());
                for (int i = 0; i < evaluators.Count; i++)
                {
                    markTypes[i].Add(SemanticProccessor.SelectBestMark(evaluators[i].Evaluate(testMsg.GetContext()[0])).Result);
                }

            } while (reader.HasNext());
            for (int i = 1; i < markTypes.Length; i++)
            {
                var matrix = new ClassifiationMatrix(markTypes[0], markTypes[i]);
                using (var output = new StreamWriter("result"))
                {
                    output.WriteLine(matrix.Summarry());
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
