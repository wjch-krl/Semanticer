namespace Semanticer.Common.Utils
{
    /// <summary>
    /// The result class the holds the analysis results
    /// </summary>
    public class DescriptiveResult
    {
        // sortedData is used to calculate percentiles
        internal double[] sortedData { get; set; }

        /// <summary>
        /// Count
        /// </summary>
        public uint Count { get; internal set; }
        /// <summary>
        /// Sum
        /// </summary>
        public double Sum { get; internal set; }
        /// <summary>
        /// Arithmatic mean
        /// </summary>
        public double Mean { get; internal set; }
        /// <summary>
        /// Geometric mean
        /// </summary>
        public double GeometricMean { get; internal set; }
        /// <summary>
        /// Harmonic mean
        /// </summary>
        public double HarmonicMean { get; internal set; }
        /// <summary>
        /// Minimum value
        /// </summary>
        public double Min { get; internal set; }
        /// <summary>
        /// Maximum value
        /// </summary>
        public double Max { get; internal set; }
        /// <summary>
        /// The range of the values
        /// </summary>
        public double Range { get; internal set; }
        /// <summary>
        /// Sample variance
        /// </summary>
        public double Variance { get; internal set; }
        /// <summary>
        /// Sample standard deviation
        /// </summary>
        public double StdDev { get; internal set; }
        /// <summary>
        /// Skewness of the data distribution
        /// </summary>
        public double Skewness { get; internal set; }
        /// <summary>
        /// Kurtosis of the data distribution
        /// </summary>
        public double Kurtosis { get; internal set; }
        /// <summary>
        /// Interquartile range
        /// </summary>
        public double IQR { get; internal set; }
        /// <summary>
        /// Median, or second quartile, or at 50 percentile
        /// </summary>
        public double Median { get; internal set; }
        /// <summary>
        /// First quartile, at 25 percentile
        /// </summary>
        public double FirstQuartile { get; internal set; }
        /// <summary>
        /// Third quartile, at 75 percentile
        /// </summary>
        public double ThirdQuartile { get; internal set; }

        /// <summary>
        /// Sum of Error
        /// </summary>
        internal double SumOfError { get; set; }

        /// <summary>
        /// The sum of the squares of errors
        /// </summary>
        internal double SumOfErrorSquare { get; set; }

        /// <summary>
        /// Percentile
        /// </summary>
        /// <param name="percent">Pecentile, between 0 to 100</param>
        /// <returns>Percentile</returns>
        public double Percentile(double percent)
        {
            return Descriptive.Percentile(sortedData, percent);
        }
    } // end of class DescriptiveResult
}