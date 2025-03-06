namespace TechStore.Application.DTOs
{
    public class ProductDto
    {
        private CategoryDto category;


        public int ProductId { get; set; }
        public int price { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CategoryDto? Category { get => category; set => category = value; }


    }
}
