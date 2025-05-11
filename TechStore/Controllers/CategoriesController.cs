using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Application.DTOs;
using TechStore.Application.Services;
using TechStore.Domain.Entities;

namespace TechStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(CategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryService.GetCategoriesAsync();

            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }
        [Authorize(Roles ="admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.UpdateCategoryAsync(category);
            _logger.LogInformation("Категория {CategoryName} измененена (Admin: {UserId})",
                category.Name, userId);
            return NoContent();
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(CategoryDto category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.AddCategoryAsync(category);
            _logger.LogInformation("Категория {CategoryName} добавлена (Admin: {UserId})",
            category.Name, userId);
            return Ok();
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _categoryService.DeleteCategoryAsync(id);
                _logger.LogInformation("Категория (Id: {id}) удалена (Admin: {UserId})",
                    id, userId);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }

        }

    }
}
