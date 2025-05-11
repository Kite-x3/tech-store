using System.ComponentModel.DataAnnotations;

namespace TechStore.Domain.Entities
{
    public class OrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } 

        [MaxLength(200)]
        public string Description { get; set; }
    }
}