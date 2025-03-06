namespace TechStore.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int price { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Category Category { get; set; }
    }
}
