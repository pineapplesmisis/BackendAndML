using System.Collections.Generic;
using MCH.Parset.Data.Entities;

namespace MCH.Data.Entities
{
    /// <summary>
    /// Сущность товара
    /// </summary>
    public class ProductEntity
    {
        public  int Id { get; set; }
        public  string ProductName { get; set; }
        public  int Price { get; set; }
        public  string Description { get; set; }
        public  int CompanyId { get; set; }
        public  string Url { get; set; }
        public virtual  CompanyEntity company { get; set; }
        public  virtual  IEnumerable<ImageEntity> Images { get; set; }
    }
}