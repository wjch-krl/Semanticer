namespace Semanticer
{
    public class SemanticResult
    {
        public SemanticType Result { get; set; }
        public double Propability { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Text} is {Result} (With propability: {Propability})";
        }
    }
}