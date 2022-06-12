using System.Collections.Generic;

namespace MCH.API.Models
{
    /// <summary>
    /// Хранит список Id товаров
    /// </summary>
    public class ProductIds
    {
        public  IEnumerable<int> Ids { get; set; }
    }
}