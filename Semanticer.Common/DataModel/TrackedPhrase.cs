namespace Semanticer.Common.DataModel
{
    public class TrackedPhrase
    {
        public TrackedPhrase()
        {
            Synonyms = new string[0];
        }

        public string Text { get; set; }
        public bool IsPhrase { get; set; }
        public bool Normalize { get; set; }
        public bool IsNormalized { get; set; }
        public int MaxDistance { get; set; }
        public int Id { get; set; }
        public bool UseSynonyms { get; set; }
        public string[] Synonyms { get; set; }
        public string FtsQuery { get; set; }
    }
}