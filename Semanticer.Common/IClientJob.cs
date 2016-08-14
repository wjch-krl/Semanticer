using System;
using Semanticer.Common.DataModel;

namespace Semanticer.Common
{
    public interface IClientJob
    {
        /// <summary>
        /// Event zgłaszany po zakończeniu zadania wykonywanego przez klienta
        /// </summary>
        event EventHandler<DiagnosticLogElement> JobCompleted;
        /// <summary>
        /// Id profilu dla którego klient został utworzony
        /// </summary>
        int ProfileId { get; }
    }
}
