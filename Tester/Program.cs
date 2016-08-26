using System;
using Semanticer.WcfClient;

namespace Tester
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var semanticProccessor = ServiceResolver.GetTrainedSemanticProccessor();
            Console.WriteLine("Enter text to calculate Semantics:");
            do
            {
                var text = Console.ReadLine();
                try
                {
                    var result = semanticProccessor.Process(text);
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
