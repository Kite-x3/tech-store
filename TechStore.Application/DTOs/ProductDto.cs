namespace TechStore.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ProductDto() { }

    }
}
