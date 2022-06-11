namespace MCH.Models
{
    public class Product
    {
        public  int Id { get; set; }
        public  string ProductName { get; set; }
        public  int Price { get; set; }
        public  string Description { get; set; }
        public  int CompanyId { get; set; }
        public  string Image { get; set; }
    }
}