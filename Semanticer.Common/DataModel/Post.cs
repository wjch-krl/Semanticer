using System;
using System.Xml.Serialization;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Posty użytkownika profilu
    /// </summary>
    public class Post : IEquatable<Post>
    {
        public int Id { get; set; }
        public string OrgId { get; set; }

        public int ProfileId { get; set; }

        public int PostParentId { get; set; }
        public int PostAuthorId { get; set; }

        public string ProfileOrgId { get; set; }
        public string PostParentOrgId { get; set; }
        public string PostAuthorOrgId { get; set; }

        public DateTime MessageDateCreate { get; set; }
        public DateTime MessageDateUpdate { get; set; }
        public DateTime DateInsert { get; set; }
        public DateTime DateUpdate { get; set; }

        public string Message { get; set; }
        public NormalizedMessage NormalizeMessage { get; set; }
        public string Lang { get; set; }

        [XmlIgnore]
        public Uri PostUri { get; set; }

        public int Level { get; set; }
        public int Strong { get; set; }
        public int Shares { get; set; }

        public bool IsNewPost { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }
        public int SourceId { get; set; }

        public SpellCheckingStatus SpellCheckingStatus { get; set; }

        public PostMarkType MarkType { get; set; }
        public double Mark { get; set; }
        public double MarkValue { get; set; }

        public bool IsProfileAuthor => PostAuthorOrgId != null && ProfileOrgId != null && PostAuthorOrgId.Equals(ProfileOrgId);

        public int RunId { get; set; }
        public int TradeId { get; set; }
        public char PostAuthorGender { get; set; }
        public bool IsTraining { get; set; }
        public string AuthorFullId => string.Join("_", PostAuthorOrgId, SourceId);
        public string FullId => string.Join("_", OrgId, SourceId);

        /// <summary>
        /// Porównuje dwa posty na podstawie id źródłowego i  treści wiadomości.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Post other)
        {
            if (OrgId.Equals(other.OrgId))
            {
                if (other.PostAuthorOrgId != null &&
                    (PostAuthorOrgId != null && PostAuthorOrgId.Equals(other.PostAuthorOrgId)))
                {
                    if (other.Message != null && Message != null && Message.Equals(other.Message))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ChangeMark(PostMarkType newMarkType)
        {
            MarkType = newMarkType;
            Mark = 2 - (int)newMarkType;
            MarkValue = Strong * Mark;
        }

        public override string ToString()
        {
            return string.Format("{1} [{0}]", OrgId, Message);
        }
    }
}