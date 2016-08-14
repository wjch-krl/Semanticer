using SharpEntropy;

namespace Semanticer.Classifier.Common
{
    /// <summary>
    /// Deklaruje właściwości niezbędne do przeprowadzenia treningu klasyfikatorów
    /// </summary>
    public interface ITrainingData
    {
        /// <summary>
        /// Obiekt dostarczający dane uczące
        /// </summary>
        ITrainingEventReader Reader { get; }
        /// <summary>
        /// Połączenie do bazy danych
        /// </summary>
        ITextAnalizerDataProvider DatabaseProvider { get; }
        /// <summary>
        /// Czy wykorzystywać ocenione słowa z bazy danych do treningu 
        /// </summary>
        bool LoadWords { get;  }
    }
}