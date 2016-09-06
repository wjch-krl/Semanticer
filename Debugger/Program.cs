using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semanticer;
using Semanticer.Classifier.Transformers.Doc2Vec;
using Semanticer.Common;
using Semanticer.TextAnalyzer;

namespace Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
//            var merger = new ImdbFileMerger("C:\\Users\\wk\\Documents\\mgr\\Semanticer\\aclImdb\\train\\train",
//                "C:\\Users\\wk\\Documents\\mgr\\Semanticer\\aclImdb\\train\\unsup");
//            merger.MergeInto(Doc2VecTransformer.FileName);
            ISemanticProccessor processor = new SemanticProccessor();
            while (!processor.IsTrained())
            {
                Task.Delay(1000).Wait();
            }
            do
            {
                EvaluateFromConsole(processor);
            } while (true);
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
}
