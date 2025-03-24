using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();

            if (!categories.Any())
            {
                throw new KeyNotFoundException("No categories found.");
            }

            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            });
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Products = category.Products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                }).ToList(),
            };
        }

        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new ArgumentNullException(nameof(categoryDto), "Category data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(categoryDto.Name));
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new ArgumentNullException(nameof(categoryDto), "Category data cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                throw new ArgumentException("Category name cannot be empty.", nameof(categoryDto.Name));
            }

            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryDto.CategoryId);

            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryDto.CategoryId} not found.");
            }

            existingCategory.Name = categoryDto.Name;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
            }

            await _categoryRepository.DeleteCategoryAsync(categoryId);
        }
    }
}
