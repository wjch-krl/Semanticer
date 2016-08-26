using System;
using System.ServiceProcess;

namespace Semanticer.Wcf
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionsHandler;
            var service = new SemanticerService();
            if (Environment.UserInteractive)
            {
                RunApp(args, service);
            }
            else
            {
                RunService(service);
            }
        }

        private static void RunApp(string[] args, SemanticerService service)
        {
            AppDomain.CurrentDomain.ProcessExit += (o, s) => service.Stop();
            service.ServiceStart();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);
        }

        private static void RunService(SemanticerService service)
        {
            ServiceBase.Run(service);
        }

        private static void UnhandledExceptionsHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            Console.WriteLine(e);
        }
    }
}