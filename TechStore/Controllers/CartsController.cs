﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechStore.Api.Models;
using TechStore.Application.DTOs;
using TechStore.Application.Services;

namespace TechStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly ILogger<CartsController> _logger;

        public CartsController(CartService cartService, ILogger<CartsController> logger)
        {
            _cartService = cartService;
            _logger = logger;   
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = User.Identity.Name;
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddItemToCart([FromBody] AddCartItemRequest request)
        {
            var userId = User.Identity.Name;
            var cart = await _cartService.AddItemToCartAsync(userId, request.ProductId, request.Quantity);
            _logger.LogInformation("Корзина обновлена (User: {UserId}, Товаров: {ItemsCount})",
            userId, cart.Items.Count);
            return Ok(cart);
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            var cart = await _cartService.UpdateCartItemAsync(cartItemId, request.Quantity);
            return Ok(cart);
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            await _cartService.RemoveCartItemAsync(cartItemId);
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.Identity.Name;
            await _cartService.ClearCartAsync(userId);
            _logger.LogInformation("Корзина очищена (User: {UserId})", userId);
            return NoContent();
        }
    }

}
