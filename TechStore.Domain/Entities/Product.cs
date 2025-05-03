using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TechStore.Domain.Entities;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Храним JSON в БД
    public string ImageUrlsJson { get; set; } = "[]";

    // Работа с json
    [NotMapped]
    public List<string> ImageUrls
    {
        get => JsonSerializer.Deserialize<List<string>>(ImageUrlsJson ?? "[]") ?? new List<string>();
        set => ImageUrlsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}