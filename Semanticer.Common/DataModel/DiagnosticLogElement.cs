using System;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    public class DiagnosticLogElement : EventArgs
    {
        public DiagnosticLogElement()
        {
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// Czas wykonania zadania
        /// </summary>
        public TimeSpan CompletitionTime { get; set; }
        /// <summary>
        /// Liczba przetworzonych elementów
        /// </summary>
        public int Processed { get; set; }
        /// <summary>
        /// Liczba znalezionych elementów
        /// </summary>
        public int Found { get; set; }
        /// <summary>
        /// Id profilu dla którego analiza jest wykonywana
        /// </summary>
        public int ProfileId { get; set; }
        /// <summary>
        /// Typ logowanego zdarzenia
        /// </summary>
        public LoggerEventType JobType { get; set; }
        /// <summary>
        /// Id nadrzędnego zdarzenia
        /// </summary>
        public int LogId { get; set; }
        /// <summary>
        /// Wiadomość do zalogowania do bazy danych
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Dodatkowe informacje do zalogowania
        /// </summary>
        public string LogDetails { get; set; }
        /// <summary>
        /// Obiekt zgłaszający zdarzenie do logowania
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// Id wpisu po zapisaniu do bazy danych
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Czas zdarzenia
        /// </summary>
        public DateTime Date { get; set; }
    }
}
