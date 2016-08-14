using System.IO;
using log4net.Core;
using log4net.Util;
using Semanticer.Common.DataModel;

namespace Semanticer.Common.Logger
{
    public class LoggableEventConverter : PatternConverter
    {
        protected override void Convert(TextWriter writer, object state)
        {
            if (state == null)
            {
                writer.Write(string.Empty);
                return;
            }

            var loggingEvent = state as LoggingEvent;
            var actionInfo = loggingEvent.MessageObject as DiagnosticLogElement;
            if (actionInfo == null)
            {
                writer.Write(string.Empty);
            }
            else
            {
                switch (Option.ToLower())
                {
                    case "completitiontime":
                        writer.Write(actionInfo.CompletitionTime.Ticks);
                        break;
                    case "found":
                        writer.Write(actionInfo.Found);
                        break;
                    case "jobtype":
                        writer.Write((int)actionInfo.JobType);
                        break;
                    case "processed":
                        writer.Write(actionInfo.Processed);
                        break;
                    case "profileid":
                        writer.Write(actionInfo.ProfileId);
                        break;
                    case "message":
                        writer.Write(actionInfo.Message);
                        break;
                    case "logdetails":
                        writer.Write(actionInfo.LogDetails);
                        break;
                    case "logid":
                        writer.Write(actionInfo.LogId);
                        break;
                    case "sender":
                        writer.Write(actionInfo.Sender);
                        break;
                    case "typedesc":
                        writer.Write(actionInfo.JobType.ToString());
                        break;                      
                    default:
                        writer.Write(string.Empty);
                        break;
                }
            }
        }
    }
}
