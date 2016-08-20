namespace Semanticer.Classifier.Doc2Vec
{
    public class ClassifiableSentence
    {
        public ClassifiableSentence(double[] features, int sentenceNumber, string orgId, int sourceId)
        {
            Features = features;
            SentenceNumber = sentenceNumber;
            OrgId = orgId;
            SourceId = sourceId;
        }

        public string OrgId { get; }
        public int SourceId { get; }
        public int SentenceNumber { get; private set; }
        public double[] Features { get;  private set; }
        public string FullId => string.Join("_", OrgId, SourceId);
    }
}