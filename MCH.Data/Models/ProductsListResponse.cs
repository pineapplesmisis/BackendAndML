using System.Collections.Generic;
using MCH.Data.Entities;

namespace MCH.API.Models
{
    /// <summary>
    /// Набор продуктов
    /// </summary>
    public class ProductsListResponse
    {
        /// <summary>
        /// Общее количетсво продуктов по данному запросу
        /// (не должно совпадать с количеством ответа при использовании count и offset)
        /// </summary>
        public  int TotalProjects { get; set; }
        public  IEnumerable<ProductEntity> products { get; set; }
    }
}