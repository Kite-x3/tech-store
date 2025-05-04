

namespace TechStore.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> products, int totalCount)> GetFilteredProductsAsync(int? categoryId,
            int pageNumber,
            int pageSize,
            string sortBy,
            bool sortDescending,
            decimal? minPrice,
            decimal? maxPrice);
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
