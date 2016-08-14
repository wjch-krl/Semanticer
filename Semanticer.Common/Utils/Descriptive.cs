using System;
using System.Linq;

namespace Semanticer.Common.Utils
{
    /// <summary>
    /// Descriptive class
    /// </summary>
    public class Descriptive
    {
        private readonly double[] data;
        private double[] sortedData;

        /// <summary>
        /// Descriptive results
        /// </summary>
        public DescriptiveResult Result = new DescriptiveResult();

        #region Constructors
        /// <summary>
        /// Descriptive analysis default constructor
        /// </summary>
        public Descriptive() { } // default empty constructor

        /// <summary>
        /// Descriptive analysis constructor
        /// </summary>
        /// <param name="dataVariable">Data array</param>
        public Descriptive(double[] dataVariable)
        {
            data = dataVariable;
        }
        #endregion //  Constructors

        /// <summary>
        /// Run the analysis to obtain descriptive information of the data
        /// </summary>
        public void Analyze()
        {
            if (data.Length == 0)
            {
                return;
            }
            // initializations
            Result.Count = 0;
            Result.Min = Result.Max = Result.Range = Result.Mean =
                Result.Sum = Result.StdDev = Result.Variance = 0.0d;

            double sumOfSquare = 0.0d;
            double sumOfESquare = 0.0d; // must initialize

            double cumProduct = 1.0d; // to calculate geometric mean
            double cumReciprocal = 0.0d; // to calculate harmonic mean

            // First iteration
            for (int i = 0; i < data.Length; i++)
            {
                if (i == 0) // first data point
                {
                    Result.Min = data[i];
                    Result.Max = data[i];
                    Result.Mean = data[i];
                    Result.Range = 0.0d;
                }
                else
                { // not the first data point
                    if (data[i] < Result.Min) Result.Min = data[i];
                    if (data[i] > Result.Max) Result.Max = data[i];
                }
                Result.Sum += data[i];
                sumOfSquare += Math.Pow(data[i], 2);

                cumProduct *= data[i];
                cumReciprocal += 1.0d / data[i];
            }

            Result.Count = (uint)data.Length;
            double n = Result.Count; // use a shorter variable in double type
            Result.Mean = Result.Sum / n;
            Result.GeometricMean = Math.Pow(cumProduct, 1.0 / n);
            Result.HarmonicMean = 1.0d / (cumReciprocal / n); // see http://mathworld.wolfram.com/HarmonicMean.html
            Result.Range = Result.Max - Result.Min;

            // second loop, calculate Stdev, sum of errors
            //double[] eSquares = new double[data.Length];
            double m1 = 0.0d;
            double m2 = 0.0d;
            double m3 = 0.0d; // for skewness calculation
            double m4 = 0.0d; // for kurtosis calculation
            // for skewness
            foreach (double t in data)
            {
                double m = t - Result.Mean;
                double mPow2 = m * m;
                double mPow3 = mPow2 * m;
                double mPow4 = mPow3 * m;

                m1 += Math.Abs(m);

                m2 += mPow2;

                // calculate skewness
                m3 += mPow3;

                // calculate skewness
                m4 += mPow4;
            }

            Result.SumOfError = m1;
            Result.SumOfErrorSquare = m2; // Added for Excel function DEVSQ
            sumOfESquare = m2;

            // var and standard deviation
            Result.Variance = sumOfESquare / ((double)Result.Count - 1);
            Result.StdDev = Math.Sqrt(Result.Variance);

            // using Excel approach
            double skewCum = 0.0d; // the cum part of SKEW formula
            for (int i = 0; i < data.Length; i++)
            {
                skewCum += Math.Pow((data[i] - Result.Mean) / Result.StdDev, 3);
            }
            Result.Skewness = n / (n - 1) / (n - 2) * skewCum;

            // kurtosis: see http://en.wikipedia.org/wiki/Kurtosis (heading: Sample Kurtosis)
            double m2_2 = Math.Pow(sumOfESquare, 2);
            Result.Kurtosis = ((n + 1) * n * (n - 1)) / ((n - 2) * (n - 3)) *
                              (m4 / m2_2) -
                              3 * Math.Pow(n - 1, 2) / ((n - 2) * (n - 3)); // second last formula for G2

            // calculate quartiles
            sortedData = new double[data.Length];
            data.CopyTo(sortedData, 0);
            Array.Sort(sortedData);

            // copy the sorted data to result object so that
            // user can calculate percentile easily
            Result.sortedData = new double[data.Length];
            sortedData.CopyTo(Result.sortedData, 0);

            Result.FirstQuartile = Percentile(sortedData, 25);
            Result.ThirdQuartile = Percentile(sortedData, 75);
            Result.Median = Percentile(sortedData, 50);
            Result.IQR = Percentile(sortedData, 75) -
                         Percentile(sortedData, 25);

        } // end of method Analyze


        /// <summary>
        /// Calculate percentile of a sorted data set
        /// </summary>
        /// <param name="sortedData"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static double Percentile(double[] sortedData, double p)
        {
            if (sortedData.Length < 2)
            {
                return sortedData.FirstOrDefault();
            }
            // algo derived from Aczel pg 15 bottom
            if (p >= 100.0d) return sortedData[sortedData.Length - 1];

            double position = (sortedData.Length + 1) * p / 100.0;
            double leftNumber = 0.0d, rightNumber = 0.0d;

            double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

            if (position >= 1)
            {
                leftNumber = sortedData[(int)Math.Floor(n) - 1];
                rightNumber = sortedData[(int)Math.Floor(n)];
            }
            else
            {
                leftNumber = sortedData[0]; // first data
                rightNumber = sortedData[1];
            }

            if (leftNumber == rightNumber)
                return leftNumber;
            double part = n - Math.Floor(n);
            return leftNumber + part * (rightNumber - leftNumber);
        } // end of internal function percentile

    }
}