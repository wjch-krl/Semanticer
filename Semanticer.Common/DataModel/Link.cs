using System;

namespace Semanticer.Common.DataModel
{
    public class Link
    {
        public Link(string url)
        {
            Url = url;
            Level = 0;
        }
        /// <summary>
        /// Absolutny adres linku.
        /// </summary>
        public string Url {get; set;}
        /// <summary>
        /// Poziom zagnieżdżenia linku na stronie.
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Id wpisu w bazie danych
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Data odwiedzenia
        /// </summary>
        public DateTime VisitTime { get; set; }

        /// <summary>
        /// Id ojca wpisu w bazie danych
        /// </summary>
        public int? ParentId { get; set; }

        public Link SimpleLink()
        {
            return new Link(Url)
            {
                Id = Id,
                Level = Level,
                VisitTime = VisitTime,
            };
        }
    }
}
