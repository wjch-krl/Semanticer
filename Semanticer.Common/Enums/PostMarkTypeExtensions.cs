namespace Semanticer.Common.Enums
{
    public static class PostMarkTypeExtensions
    {
        public static PostMarkType ToPostMarkType(this double value)
        {
            if (value < 2.0 && value > -1.2)
            {
                return PostMarkType.Neutral;
            }
            if (value < 0)
            {
                return PostMarkType.Negative;
            }
            return PostMarkType.Positive;
        }
    }
}