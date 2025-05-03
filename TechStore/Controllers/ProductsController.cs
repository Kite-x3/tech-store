using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TechStore.Application.DTOs;
using TechStore.Application.Services;
using TechStore.Domain.Entities;

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
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsAsync(
            [FromQuery] int? categoryId, 
            [FromQuery] string? name)
        {
            var products = await _productService.GetProductsAsync(categoryId, name);
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        //[Authorize(Roles = "admin")] 
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


        //[Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, ProductDto product,
            [FromForm] List<IFormFile> newImages = null,
            [FromForm] List<string> imagesToDelete = null)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            await _productService.UpdateProductAsync(product, newImages, imagesToDelete);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        //[Authorize(Roles = "admin")]
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
