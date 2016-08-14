using System;
using System.Linq;
using System.Text;
using Semanticer.Common.Enums;

namespace Semanticer.Classifier.Common
{
    public class ClassifiationMatrix
    {
        private readonly int[][] matrix;

        public ClassifiationMatrix(PostMarkType[] predictions, PostMarkType[] values)
        {
            if (predictions.LongLength != values.LongLength)
            {
                throw new ArgumentException("Arrays must be same size");
            }
            matrix = new int[3][];
            for (int index = 0; index < matrix.Length; index++)
            {
                matrix[index] = new int[3];
            }
            for (int i = 0; i < predictions.Length; i++)
            {
                int predIndex = (int) (predictions[i]) - 1;
                int valueIndex = (int) (values[i]) - 1;
                if (predIndex >= 0)
                    matrix[predIndex][valueIndex]++;
            }
        }

        public double OverallPrecision()
        {
            double diagonalSum = matrix[0][0] + matrix[1][1] + matrix[2][2];
            return diagonalSum/matrix.Sum(x => x.Sum());
        }

        public double Precision(PostMarkType mark)
        {
            int index = (int) mark - 1;
            return (double) matrix[index][index]/(matrix[0][index]+matrix[1][index]+matrix[2][index]);
        }

        public double VeryBadPrecent()
        {
            return (double) (matrix[0][2] + matrix[2][0])/matrix.Sum(x => x.Sum());
        }

        public string Summarry()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("\t{0}\n", String.Join("\t", new[] { 1, 2, 3 }));
            for (int i = 0; i < matrix.Length; i++)
            {
                builder.Append(i + 1);
                builder.AppendFormat("\t{0}\n", String.Join("\t", matrix[i]));
            }
            builder.AppendFormat("Precision: {0}%\tPositive precision: {1}%\tNeutral precision: {2}%\tNegative precision:" +
                                 " {3}%\tVery bad: {4}%\n",
                OverallPrecision() * 100, Precision(PostMarkType.Positive) * 100, Precision(PostMarkType.Neutral) * 100, 
                Precision(PostMarkType.Negative) * 100, VeryBadPrecent() * 100);
            return builder.ToString();
        }

        public override string ToString()
        {
            var rows = matrix.Select(x => string.Format("{{{0}}}",string.Join(",", x)));
            return string.Format("{{{0}}}", string.Join(",", rows));
        }
    }
}