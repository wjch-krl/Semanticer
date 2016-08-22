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
    }
}