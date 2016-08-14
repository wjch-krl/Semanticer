using log4net.Layout;
using log4net.Util;

namespace Semanticer.Common.Logger
{
    class JobCompleteLayoutPattern : PatternLayout
    {
        public JobCompleteLayoutPattern()
        {
            AddConverter(new ConverterInfo
                {
                    Name = "actionInfo",
                    Type = typeof(LoggableEventConverter)
                }
            );
        }
    }
}
