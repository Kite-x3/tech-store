using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId, string? name)
        {
            var products = await _productRepository.GetProductsAsync(categoryId, name);
            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                description = p.description,
                price = p.price,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            });
        }
        public async Task CreateProductAsync(ProductDto product)
        {
            var newProduct = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                description = product.description,
                price = product.price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
            };
            await _productRepository.CreateProductAsync(newProduct);
        }
        public async Task UpdateProductAsync(ProductDto product)
        {
            var updatedProduct = new Product
            {
                ProductId = product.ProductId,
                Name = product.Name,
                description = product.description,
                price = product.price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
            };
            await _productRepository.UpdateProductAsync(updatedProduct);
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }
    }
}
