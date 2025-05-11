using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TechStore.Application.DTOs;
using TechStore.Application.Services;

namespace TechStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<ProductDto>>> GetProductsByCategory(
            [FromQuery] ProductQueryParams queryParams)
        {
            var result = await _productService.GetProductsByCategoryAsync(queryParams);
            return Ok(result);
        }

        [HttpGet("main")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsAsync()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [Authorize(Roles = "admin")] 
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto request)
        {
            try
            {
                // Валидация входных данных
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Получаем файлы изображений
                var imageFiles = Request.Form.Files
                    .Where(f => f.Name.StartsWith("image")) // соответствие клиентской части
                    .ToList();

                // Создаем DTO для сервиса
                var productDto = new ProductDto
                {
                    ProductName = request.ProductName,
                    Description = request.Description,
                    Price = request.Price,
                    CategoryId = request.CategoryId
                };

                // Вызываем сервис
                await _productService.CreateProductAsync(productDto, imageFiles);

                // Формируем ответ с DTO созданного продукта
                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = productDto.Id },
                    productDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProductAsync(
            int id,
            [FromForm] ProductDto request,
            [FromForm] string? imagesToDelete = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != request.Id)
                return BadRequest("ID mismatch");

            try
            {
                // Получаем новые изображения
                var newImageFiles = Request.Form.Files
                    .Where(f => f.Name.StartsWith("image"))
                    .ToList();

                // Десериализуем imagesToDelete
                List<string>? imagesToDeleteList = null;
                if (!string.IsNullOrEmpty(imagesToDelete))
                {
                    imagesToDeleteList = JsonSerializer.Deserialize<List<string>>(imagesToDelete);
                }

                await _productService.UpdateProductAsync(request, newImageFiles, imagesToDeleteList);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = request.Id },
                    request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
