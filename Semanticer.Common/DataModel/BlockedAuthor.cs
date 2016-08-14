namespace Semanticer.Common.DataModel
{
    public class BlockedAuthor
    {
        public int Id { get; set; }
        public string OrgId { get; set; }
        public string Name { get; set; }
        public int ProfileId { get; set; }
        public int? AuthorId { get; set; }
        public int SourceId { get; set; }
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ? OrgId : Name;
        }
    }
}
