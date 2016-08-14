using System;
using System.Xml.Serialization;
using Semanticer.Common.Enums;

namespace Semanticer.Common.DataModel
{
    /// <summary>
    /// Serializowana do XML klasa z ustawieniami konfiguracyjnymi dostępu do źródła dla danego profilu
    /// </summary>
    [Serializable]
    [XmlInclude(typeof (TwitterProfileData))]
    [XmlInclude(typeof (EmailProfileData))]
    [XmlInclude(typeof (CrawlerProfileData))]
    public class ProfileData
    {
        /// <summary>
        /// Typ źródła
        /// </summary>
        public SourceType ProfileSourceType { get; set; }

        /// <summary>
        /// Poszukiwane słowa kluczowe
        /// </summary>
        public string[] Keywords { get; set; }

        /// <summary>
        /// Bezparametrowy konstruktor do serializacji
        /// </summary>
        public ProfileData()
        {
        }

        /// <summary>
        /// Maksymalna ilość pasujących elementów
        /// </summary>
        public int MathCount { get; set; }

        /// <summary>
        /// Maksymalna ilość przetworzonych elementów
        /// </summary>
        public int ProcessedCount { get; set; }

        /// <summary>
        /// Data od której obierane są tylko nowsze posty
        /// </summary>
        public DateTime LastPostDate { get; set; }

        /// <summary>
        /// Ignoruje ograniczenia ilości i czasu postów
        /// </summary>
        public bool GetAll { get; set; }

        /// <summary>
        /// Czy poprawiać znalezione błędy pisowni
        /// </summary>
        public bool CorrectSpelling { get; set; }

        public static ProfileData Default => new ProfileData
        {
            ProfileSourceType = SourceType.FACEBOOK,
            MathCount = -1,
            ProcessedCount = -1,
            GetAll = true
        };

        /// <summary>
        /// Standardowy konstruktor
        /// </summary>
        /// <param name="sourceType"> Typ źródła </param>
        /// <param name="keywords"> Słowa kluczowe</param>
        public ProfileData(SourceType sourceType, string[] keywords)
        {
            ProfileSourceType = sourceType;
            Keywords = keywords;
        }

        public override bool Equals(object obj)
        {
            ProfileData pd = obj as ProfileData;
            if (pd == null)
            {
                return false;
            }
            return pd.CorrectSpelling == CorrectSpelling && pd.GetAll == GetAll &&
                   pd.MathCount == MathCount && pd.ProcessedCount == ProcessedCount &&
                   pd.ProfileSourceType == ProfileSourceType && GetType() == pd.GetType();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                const int hashingBase = (int)2166136261;
                const int hashingMultiplier = 16777619;

                int hash = hashingBase;
                hash = (hash * hashingMultiplier) ^ CorrectSpelling.GetHashCode();
                hash = (hash * hashingMultiplier) ^ GetAll.GetHashCode();
                hash = (hash*hashingMultiplier) ^ MathCount.GetHashCode();
                hash = (hash * hashingMultiplier) ^ MathCount.GetHashCode();
                hash = (hash * hashingMultiplier) ^ ProfileSourceType.GetHashCode();
                return hash;
            }
        }
    }
}