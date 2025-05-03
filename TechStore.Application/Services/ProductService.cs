using Microsoft.AspNetCore.Http;
using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ImageService _imageService;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ImageService imageService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
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
                ImageUrls = p.ImageUrls
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
                ImageUrls = product.ImageUrls,
            };
        }

        public async Task CreateProductAsync(ProductDto productDto, List<IFormFile> imageFiles = null)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto));

            var imageUrls = new List<string>();
            if (imageFiles != null)
            {
                foreach (var imageFile in imageFiles)
                {
                    var imageUrl = await _imageService.SaveImageAsync(imageFile);
                    imageUrls.Add(imageUrl);
                }
            }

            var newProduct = new Product
            {
                Name = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                ImageUrls = imageUrls 
            };

            await _productRepository.CreateProductAsync(newProduct);

            productDto.Id = newProduct.ProductId;
            productDto.ImageUrls = newProduct.ImageUrls;
            productDto.CreatedAt = newProduct.CreatedAt;
            productDto.UpdatedAt = newProduct.UpdatedAt;
        }



        public async Task UpdateProductAsync(ProductDto productDto, List<IFormFile> newImages = null, List<string> imagesToDelete = null)
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

            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (var imageUrl in imagesToDelete)
                {
                    await _imageService.DeleteImageAsync(imageUrl);
                    existingProduct.ImageUrls.Remove(imageUrl);
                }
            }

            if (newImages != null && newImages.Any())
            {
                foreach (var imageFile in newImages)
                {
                    var imageUrl = await _imageService.SaveImageAsync(imageFile);
                    existingProduct.ImageUrls.Add(imageUrl);
                }
            }

            existingProduct.Name = productDto.ProductName;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.CategoryId = productDto.CategoryId;

            await _productRepository.UpdateProductAsync(existingProduct);
            productDto.ImageUrls = existingProduct.ImageUrls;
            productDto.UpdatedAt = existingProduct.UpdatedAt;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found.");
            }

            foreach (var imageUrl in existingProduct.ImageUrls)
            {
                await _imageService.DeleteImageAsync(imageUrl);
            }

            await _productRepository.DeleteProductAsync(productId);
        }
    }
}
