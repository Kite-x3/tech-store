using TechStore.Application.DTOs;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;

namespace TechStore.Application.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }
        /// <summary>
        /// Получает корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>DTO корзины с товарами</returns>
        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    Product = new ProductDto
                    {
                        Id = i.Product.ProductId,
                        ProductName = i.Product.Name,
                        Price = i.Product.Price,
                        ImageUrls = i.Product.ImageUrls
                    },
                    Quantity = i.Quantity
                }).ToList()
            }; ;
        }
        /// <summary>
        /// Добавляет товар в корзину
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <param name="quantity">Количество (по умолчанию 1)</param>
        /// <returns>Обновленное DTO корзины</returns>
        public async Task<CartDto> AddItemToCartAsync(string userId, int productId, int quantity = 1)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            Cart cart;

            try
            {
                cart = await _cartRepository.GetCartByUserIdAsync(userId);
            }
            catch (KeyNotFoundException)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.AddCartAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = productId,
                    CartId = cart.CartId,
                    Quantity = quantity
                };
                await _cartRepository.AddCartItemAsync(newItem);
            }

            return await GetCartByUserIdAsync(userId);
        }
        /// <summary>
        /// Обновляет количество товара в корзине
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <param name="quantity">Новое количество</param>
        /// <returns>Обновленное DTO корзины</returns>
        public async Task<CartDto> UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var item = await _cartRepository.GetCartItemAsync(cartItemId);
            item.Quantity = quantity;
            await _cartRepository.UpdateCartItemAsync(item);
            return await GetCartByUserIdAsync(item.Cart.UserId);
        }
        /// <summary>
        /// Удаляет товар из корзины
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        public async Task RemoveCartItemAsync(int cartItemId)
        {
            await _cartRepository.RemoveCartItemAsync(cartItemId);
        }
        /// <summary>
        /// Очищает корзину пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public async Task ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            await _cartRepository.ClearCartAsync(cart.CartId);
        }
    }
}
