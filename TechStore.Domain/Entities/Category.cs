﻿using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.Entities
{
    public class Category
    {
        [Key]

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
