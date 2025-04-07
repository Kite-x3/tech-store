using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsAsync([FromQuery] int? categoryId, [FromQuery] string? name)
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
        public async Task<ActionResult<ProductDto>> CreateProductAsync(ProductDto product)
        {
            await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
        //[Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, ProductDto product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }
            await _productService.UpdateProductAsync(product);
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
