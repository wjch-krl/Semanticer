using System.Threading;

namespace Semanticer.Common.Utils
{
    public class Counter
    {
        private readonly int maxValue;
        private int value;

        public Counter(int maxValue)
        {
            this.maxValue = maxValue;
        }

        public static Counter operator ++(Counter counter)
        {
            Interlocked.Increment(ref counter.value);
            if (counter.value > counter.maxValue)
            {
                Interlocked.Exchange(ref counter.value, 0);
            }
            return counter;
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
