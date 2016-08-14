using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Config;
using Semanticer.Common.DataModel;
using Semanticer.Common.Enums;

namespace Semanticer.Common.Logger
{
    public class LoggProvider
    {
        private static readonly ILog _logger;
        private readonly object client;
        private readonly int profileId;
        private readonly int runId;
        private static readonly ILog _errorLogger;

        static LoggProvider()
        {
            LoadLoggerXmlConfig(String.Format("{0}{1}",AppDomain.CurrentDomain.BaseDirectory,"\\log4net.xml"));
            _errorLogger = LogManager.GetLogger("ErrorLogger");
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        
      
        }

        public static void LoadLoggerXmlConfig(string path)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                XmlConfigurator.Configure(file);
            }
            catch (FileNotFoundException)
            {
                BasicConfigurator.Configure();
            }
        }

        public LoggProvider(object client, int profileId, int runId)
        {
            this.client = client;
            this.profileId = profileId;
            this.runId = runId;
        }

        public LoggProvider(IClientJob client, int runId)
        {
            profileId = client.ProfileId;
            this.client = client;
            this.runId = runId;
            client.JobCompleted += client_JobCompleted;
        }

        public DateTime LoggStart(LoggerEventType type, string message)
        {
            _logger.Debug(new DiagnosticLogElement
            {
                JobType = type,
                ProfileId = profileId,
                Message = message,
                LogId = runId,
                CompletitionTime = default(TimeSpan),
            });
            return DateTime.UtcNow;
        }

        public static DateTime LoggStart(LoggerEventType type, string message,int profileId, int runId)
        {
            _logger.Debug(new DiagnosticLogElement
            {
                JobType = type,
                ProfileId = profileId,
                Message = message,
                LogId = runId,
                CompletitionTime = default(TimeSpan),
            });
            return DateTime.UtcNow;
        }

        public void LoggStop(DiagnosticLogElement args)
        {
            args.LogId = runId;
            args.ProfileId = profileId;
            args.Sender = client.GetType().ToString();
            _logger.Debug(args);
        }

        public static void LoggError(int profileId, string message, Exception exception)
        {
            _errorLogger.Error(new DiagnosticLogElement
            {
                ProfileId = profileId,
                Message = message,
                LogDetails = exception.ToString(),
            });
        }

        private void client_JobCompleted(object sender, DiagnosticLogElement args)
        {
            args.LogId = runId;
            args.ProfileId = profileId;
            args.Sender = sender.GetType().ToString();
            _logger.Info(args);
        }

        public static void LoggErrorSimple(string message, int profileId)
        {
            _errorLogger.Error(new DiagnosticLogElement
            {
                ProfileId = profileId,
                Message = message,
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            var method = sf.GetMethod();
            if (method.DeclaringType != null)
                return string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            return method.Name;
        }
    }
}
 