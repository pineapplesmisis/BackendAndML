using System.Collections;
using System.Collections.Generic;
using MCH.Data.Entities;

namespace MCH.Parset.Data.Entities
{
    /// <summary>
    /// Сущность компании
    /// </summary>
    public class CompanyEntity
    {
        public  int Id { get; set; }
        public  string CompanyName { get; set; }
        public  string IIN { get; set; }
        public  string Url { get; set; }
        public  virtual  IEnumerable<ProductEntity > products { get; set; }
        public  virtual  IEnumerable<UrlsToParseEntity> urls { get; set; }
    }
}