namespace Semanticer.Common.Enums
{
    public static class MarkTypeExtensions
    {
        public static MarkType ToMarkType(this double value)
        {
            if (value < 2.0 && value > -1.2)
            {
                return MarkType.Neutral;
            }
            if (value < 0)
            {
                return MarkType.Negative;
            }
            return MarkType.Positive;
        }
    }
}