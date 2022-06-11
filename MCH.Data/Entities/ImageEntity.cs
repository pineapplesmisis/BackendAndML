using System.Text.Json.Serialization;

namespace MCH.Data.Entities
{
    public class ImageEntity
    {
        [JsonIgnore]
        public  int Id { get; set; }
        public  string Url { get; set; }
        [JsonIgnore]
        public  int ProductId { get; set; }
       [JsonIgnore]
        public virtual  ProductEntity ProductEntity { get; set; }
    }
}