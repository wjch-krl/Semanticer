using System;
using Semanticer;
using Semanticer.WcfClient;

namespace Tester
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var semanticProccessor = ServiceResolver.GetSemanticProcessor();
            var processorHelper = new SemanticerServiceHelper(semanticProccessor);
            Console.WriteLine("Enter text to calculate Semantics:");
            do
            {
                var text = Console.ReadLine();
                try
                {
                    var result = processorHelper.Proccessor.Process(text);
                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (true);
        }
    }
}
