using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechStore.Application.DTOs;
using TechStore.Application.Services;
using TechStore.Domain.Entities;

namespace TechStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByProductId(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetById(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> Create([FromBody] ReviewDto reviewDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                await _reviewService.CreateReviewAsync(reviewDto, userId);
                _logger.LogInformation("Отзыв ID {ReviewId} создан (User: {UserId}, Продукт: {ProductId})",
            reviewDto.Id, userId, reviewDto.ProductId);
                return CreatedAtAction(nameof(GetById), new { id = reviewDto.Id }, reviewDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _reviewService.DeleteReviewAsync(id);
                _logger.LogInformation("Отзыв ID {ReviewId} удален (Admin: {UserId})", id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}