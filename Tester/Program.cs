using System;
using Tester.Semanticer.Wcf;

namespace Tester
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var semanticProccessor = new SemanticProccessorServiceClient();
            Console.WriteLine("Enter text to calculate Semantics:");
            do
            {
                var text = Console.ReadLine();
                var result = semanticProccessor.Process(text);
                Console.WriteLine(result);
            } while (true);
        }
    }
}
