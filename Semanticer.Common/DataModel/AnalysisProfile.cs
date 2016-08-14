namespace Semanticer.Common.DataModel
{
    public class AnalysisProfile
    {
        public int EntryId { get; set; }
        public string Name { get; set; }
        public int AnalId { get; set; }
        public int? ProfileId { get; set; }
        public int? SourceId { get; set; }
        public bool IncludeNonMatches { get; set; }
    }
}
