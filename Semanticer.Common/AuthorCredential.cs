namespace Semanticer.Common
{
    /// <summary>
    /// Klasa przechowująca informacje  na facebook'u.
    /// </summary>
    public class AuthorCredential
    {
        /// <summary>
        /// Nazwa strony.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Token strony (umożliwia np. dodawanie postów, odpowiedzi do postów w imieniu strony.
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// Unikalne w obrębie jednej, zarejestrowanej aplikacji na facebook'u.
        /// </summary>
        public string Id { get; set; }

        override public string ToString()
        {
            return Name;
        }
    }
}
