using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Semanticer.Wcf
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionsHandler;
            var service = new SemanticerService();
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(service);
            }
            else
            {
                AppDomain.CurrentDomain.ProcessExit += (o, s) => service.Stop();
                service.ServiceStart(args);
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);
            }
        }

        private static void UnhandledExceptionsHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            Console.WriteLine(e);
        }
    }
}