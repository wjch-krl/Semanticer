using System;

namespace Semanticer.Classifier.Vector
{
    public class VectorUtils
    {
        public static int ScalarProduct(int[] one, int[] two)
        {
            if ((one == null) || (two == null))
            {
                throw new ArgumentNullException("Arguments cannot be null");
            }

            if (one.Length != two.Length)
            {
                throw new ArgumentException("Arguments of different length are not allowed");
            }

            int result = 0;
            for (int i = 0; i < one.Length; i++)
            {
                result += one[i]*two[i];
            }
            return result;
        }

        public static double VectorLength(int[] vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException("Arguments cannot be null");
            }

            double sumOfSquares = 0d;
            for (int i = 0; i < vector.Length; i++)
            {
                sumOfSquares = sumOfSquares + (vector[i]*vector[i]);
            }

            return Math.Sqrt(sumOfSquares);
        }

        public static double CosineOfVectors(int[] one, int[] two)
        {
            if ((one == null) || (two == null))
            {
                throw new ArgumentNullException("Arguments cannot be null");
            }

            if (one.Length != two.Length)
            {
                throw new ArgumentException("Arguments of different length are not allowed");
            }
            double denominater = (VectorLength(one)*VectorLength(two));
            if (denominater == 0)
            {
                return 0;
            }
            return (ScalarProduct(one, two)/denominater);
        }
    }
}
