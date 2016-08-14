using System;
using System.Collections.Generic;
using Semanticer.Common.DataModel;

namespace Semanticer.Common.Utils
{
    public sealed class PostAggreagateEqualityComparer<T> : IEqualityComparer<PostAggreagate<T>>
    {
        public bool Equals(PostAggreagate<T> x, PostAggreagate<T> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.AnaliticsId == y.AnaliticsId &&
                string.Equals(x.AuthorOrgid, y.AuthorOrgid) && x.Date.Equals(y.Date) && 
                x.Gender == y.Gender && string.Equals(x.Lang, y.Lang) &&
                x.Level == y.Level && x.Month.Equals(y.Month) && x.PostMarkType == y.PostMarkType && 
                string.Equals(x.PostOrgid, y.PostOrgid) && x.ProfileId == y.ProfileId &&
                x.SourceId == y.SourceId && string.Equals(x.Word, y.Word);
        }

        public int GetHashCode(PostAggreagate<T> obj)
        {
            unchecked
            {
                var hashCode = obj.AnaliticsId.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.AuthorOrgid != null ? obj.AuthorOrgid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Date.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Gender.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Lang != null ? obj.Lang.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Level.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Month.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.PostMarkType.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.PostOrgid != null ? obj.PostOrgid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.ProfileId;
                hashCode = (hashCode * 397) ^ obj.SourceId.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.Word != null ? obj.Word.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class PostAggreagate<T>
    {
        public T Data { get; set; }
        public int ProfileId { get; set; }
        public string PostOrgid { get; set; }
        public DateTime? Date { get; set; }
        public int? SourceId { get; set; }
        public int? PostMarkType { get; set; }
        public string Lang { get; set; }
        public int? AnaliticsId { get; set; }
        public char? Gender { get; set; }
        public string Word { get; set; }
        public string AuthorOrgid { get; set; }
        public DateTime? Month { get; set; }
        public int? Level { get; set; }

        public PostAggreagate()
        {
        }

        /// <summary>
        /// Default aggregate with post values
        /// </summary>
        /// <param name="post"></param>
        /// <param name="analId"></param>
        public PostAggreagate(Post post)
        {
            ProfileId = post.ProfileId;
            Month = post.MessageDateCreate.ToMonthDate();
            Gender = post.PostAuthorGender;
            PostMarkType = (int) post.MarkType;
            Date = post.MessageDateCreate.Date;
            Level = post.Level;
        } 

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="previusAggreagate"></param>
        public PostAggreagate(PostAggreagate<T> previusAggreagate)
        {
            AnaliticsId = previusAggreagate.AnaliticsId;
            AuthorOrgid = previusAggreagate.AuthorOrgid;
            Data = previusAggreagate.Data;
            Date = previusAggreagate.Date;
            Gender = previusAggreagate.Gender;
            Lang = previusAggreagate.Lang;
            Month = previusAggreagate.Month;
            PostMarkType = previusAggreagate.PostMarkType;
            PostOrgid = previusAggreagate.PostOrgid;
            ProfileId = previusAggreagate.ProfileId;
            SourceId = previusAggreagate.SourceId;
            Word = previusAggreagate.Word;
            Level = previusAggreagate.Level;
        }    
    }
}