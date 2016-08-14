using System;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    [Serializable]
    public class EmailProfileData : AuthorizedProfileData
    {
        /// <summary>
        /// Typ serwera e-mail
        /// </summary>
        public EmailType EmailType { get; set; }

        /// <summary>
        /// Nazwa źródła
        /// </summary>
        public string ServerAdress { get; set; }

        /// <summary>
        /// Adres e-mail
        /// </summary>
        public string Address { get; set; }

         /// <summary>
        /// Bezparametrowy konstruktor do serializacji
        /// </summary>
        public EmailProfileData()
        {
        }

        public int ServerPort { get; set; }

        /// <summary>
        /// Standardowy konstruktor
        /// </summary>
        /// <param name="sourceType"> Typ źródła </param>
        /// <param name="serverAdress"> Nazwa źródła </param>
        /// <param name="emailType"> Typ serwera e-mail </param>
        /// <param name="address"> Adres e-mail </param>
        /// <param name="passwordData"> Zaszyfrowane hasło do konta e-mail </param>
        public EmailProfileData(SourceType sourceType, string[] keywords, EmailType emailType, string address, byte[] passwordData)
            : base(sourceType, keywords)
        {
            EmailType = emailType;
            Address = string.IsNullOrEmpty(address) ? string.Empty : address;
            PasswordData = passwordData ?? new byte[0];
        }
    }
}
