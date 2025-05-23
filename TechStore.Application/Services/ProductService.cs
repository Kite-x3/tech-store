﻿using Microsoft.AspNetCore.Http;
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
        /// <summary>
        /// Получает товары по категории с фильтрацией и пагинацией
        /// </summary>
        /// <param name="queryParams">Параметры запроса</param>
        /// <returns>Пагинированный список DTO товаров</returns>
        /// <exception cref="KeyNotFoundException">Если категория не найдена</exception>
        public async Task<PaginatedResponse<ProductDto>> GetProductsByCategoryAsync(
            ProductQueryParams queryParams)
        {
            if (queryParams.CategoryId.HasValue &&
            !await _categoryRepository.ExistsAsync(queryParams.CategoryId.Value))
            {
                throw new KeyNotFoundException($"Category with id {queryParams.CategoryId} not found");
            }

            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(
                queryParams.CategoryId,
                queryParams.PageNumber,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortDescending,
                queryParams.MinPrice,
                queryParams.MaxPrice);

            return new PaginatedResponse<ProductDto>
            {
                Items = products.Select(p => new ProductDto
                {
                    Id = p.ProductId,
                    ProductName = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CategoryId = p.CategoryId,
                    ImageUrls = p.ImageUrls
                }),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }
        /// <summary>
        /// Получает список товаров
        /// </summary>
        /// <returns>Список DTO товаров</returns>
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _productRepository.GetProductsAsync();

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
        /// <summary>
        /// Получает товар по ID
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>DTO товара</returns>
        /// <exception cref="KeyNotFoundException">Если товар не найден</exception>
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
        /// <summary>
        /// Создает новый товар
        /// </summary>
        /// <param name="productDto">DTO с данными товара</param>
        /// <param name="imageFiles">Файлы изображений (опционально)</param>
        /// <exception cref="ArgumentNullException">Если данные товара не указаны</exception>
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


        /// <summary>
        /// Обновляет данные товара
        /// </summary>
        /// <param name="productDto">DTO с обновленными данными</param>
        /// <param name="newImages">Новые изображения (опционально)</param>
        /// <param name="imagesToDelete">Изображения для удаления (опционально)</param>
        /// <exception cref="ArgumentNullException">Если данные товара не указаны</exception>
        /// <exception cref="ArgumentException">Если название товара пустое</exception>
        /// <exception cref="KeyNotFoundException">Если товар или категория не найдены</exception>
        public async Task UpdateProductAsync(ProductDto productDto, List<IFormFile>? newImages = null, List<string>? imagesToDelete = null)
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

            var currentImageUrls = existingProduct.ImageUrls.ToList();

            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (var imageUrl in imagesToDelete)
                {
                    await _imageService.DeleteImageAsync(imageUrl);
                    currentImageUrls.Remove(imageUrl); 
                }
            }

            var newImageUrls = new List<string>();
            if (newImages != null && newImages.Count > 0)
            {
                foreach (var imageFile in newImages)
                {
                    var imageUrl = await _imageService.SaveImageAsync(imageFile);
                    newImageUrls.Add(imageUrl);
                }
                currentImageUrls.AddRange(newImageUrls);
            }

            existingProduct.Name = productDto.ProductName;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            existingProduct.ImageUrls = currentImageUrls;

            await _productRepository.UpdateProductAsync(existingProduct);

            productDto.ImageUrls = existingProduct.ImageUrls;
            productDto.UpdatedAt = existingProduct.UpdatedAt;
        }
        /// <summary>
        /// Удаляет товар
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <exception cref="KeyNotFoundException">Если товар не найден</exception>
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
