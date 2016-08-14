using System.Xml.Serialization;
using Semanticer.Common.Enums;
using Semanticer.Common.Utils;

namespace Semanticer.Common.DataModel
{
    public abstract class AuthorizedProfileData: ProfileData
    {
        public AuthorizedProfileData(SourceType sourceType, string[] keywords)
            : base(sourceType, keywords)
        {
            
        }

        public AuthorizedProfileData()
        {
         
        }

        /// <summary>
        /// Nazwa użytkownika (login)
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Zaszyfrowane hasło
        /// </summary>
        public byte[] PasswordData { get; set; }

        /// <summary>
        /// Hasło
        /// </summary>
        [XmlIgnore]
        public string Password
        {
            get { return EmailPasswordHasher.DecryptPassword(PasswordData); }
            set { PasswordData = EmailPasswordHasher.EncryptPassword(value); }
        }
    }
}
