using System.ComponentModel.DataAnnotations;

namespace TechStore.Application.DTOs
{
    public class ProductQueryParams
    {
        public int? CategoryId { get; set; }
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "name";
        public bool SortDescending { get; set; } = false;

        [Range(0, int.MaxValue)]
        public decimal? MinPrice { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? MaxPrice { get; set; }
    }
}
