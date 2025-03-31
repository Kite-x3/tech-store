using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId, string? name)
        {
            var products = await _productRepository.GetProductsAsync(categoryId, name);

            return products.Select(p => new ProductDto
            {
                Id = p.ProductId,
                ProductName = p.Name,
                Description = p.Description,
                Price = p.Price,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryId = p.CategoryId,
            });
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId)
                ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");

            return new ProductDto
            {
                Id = product.ProductId,
                ProductName = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                CategoryId = product.CategoryId,
            };
        }

        public async Task CreateProductAsync(ProductDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");

            if (string.IsNullOrWhiteSpace(productDto.ProductName))
                throw new ArgumentException("Product name cannot be empty.", nameof(productDto.ProductName));

            if (!await _categoryRepository.ExistsAsync(productDto.CategoryId))
                throw new ArgumentException($"Category with ID {productDto.CategoryId} does not exist.", nameof(productDto.CategoryId));

            var newProduct = new Product
            {
                Name = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = productDto.CategoryId
            };

            await _productRepository.CreateProductAsync(newProduct);
            productDto.Id = newProduct.ProductId;
        }



        public async Task UpdateProductAsync(ProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "Product data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(productDto.ProductName))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(productDto.ProductName));
            }

            var existingProduct = await _productRepository.GetProductByIdAsync(productDto.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");
            }

            if (!await _categoryRepository.ExistsAsync(productDto.CategoryId))
            {
                throw new ArgumentException($"Category with ID {productDto.CategoryId} does not exist.", nameof(productDto.CategoryId));
            }

            existingProduct.Name = productDto.ProductName;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.CategoryId = productDto.CategoryId;

            await _productRepository.UpdateProductAsync(existingProduct);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            await _productRepository.DeleteProductAsync(productId);
        }
    }
}
