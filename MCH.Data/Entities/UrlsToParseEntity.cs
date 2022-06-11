using MCH.Parset.Data.Entities;

namespace MCH.Data.Entities
{
    /// <summary>
    /// Список ссылок с которых будет начинаться срез
    /// </summary>
    public class UrlsToParseEntity
    {
        public  int Id { get; set; }
        public  int CompanyId { get; set; }
        public virtual CompanyEntity Company { get; set; }
        public string Url { get; set; }
    }
}