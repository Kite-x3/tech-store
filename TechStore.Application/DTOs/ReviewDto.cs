namespace TechStore.Application.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
    }
}