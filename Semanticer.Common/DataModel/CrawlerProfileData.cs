namespace Semanticer.Common.DataModel
{
    public class CrawlerProfileData : AuthorizedProfileData
    {
        /// <summary>
        /// Ścieżka xPath wskazująca na element do wpisywania loginu
        /// </summary>
        public string LoginInputXPath { get; set; }

        /// <summary>
        /// Ścieżka xPath wskazująca na element do wpisywania hasła
        /// </summary>
        public string PassInputXPath { get; set; }

        /// <summary>
        /// Ścieżka xPath wskazująca na element do wysłania formularza
        /// </summary>
        public string SubmitXpath { get; set; }

        /// <summary>
        /// Maksymalny poziom zagłębienia linków
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Czy crawling odbywa się jedynie w obrębie domeny
        /// </summary>
        public bool OnlyInDomain { get; set; }

        /// <summary>
        /// Ścieżka do wzorca XML który ma zostać użyty przy analizie forum w bazie danych
        /// </summary>
        public int? TagDictId { get; set; }
    }
}
