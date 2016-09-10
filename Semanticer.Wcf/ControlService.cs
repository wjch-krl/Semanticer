using Semanticer.Common;

namespace Semanticer.Wcf
{
    public class ControlService : IControlService
    {
        public bool IsAlive()
        {
            return true;
        }
    }
}
