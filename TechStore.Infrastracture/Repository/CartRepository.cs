using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastracture.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Получает корзину пользователя по ID пользователя (создает новую, если не существует)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Корзина пользователя</returns>
        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
        /// <summary>
        /// Получает элемент корзины по ID с включенными связанными данными (товар, корзина)
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        /// <returns>Найденный элемент корзины</returns>
        /// <exception cref="KeyNotFoundException">Если элемент не найден</exception>
        public async Task<CartItem> GetCartItemAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(i => i.Product)
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId)
                ?? throw new KeyNotFoundException("Cart item not found");
        }
        /// <summary>
        /// Добавляет новую корзину в базу данных
        /// </summary>
        /// <param name="cart">Объект корзины для создания</param>
        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Добавляет новый элемент в корзину
        /// </summary>
        /// <param name="item">Объект элемента корзины для добавления</param>
        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Обновляет информацию об элементе корзины
        /// </summary>
        /// <param name="item">Объект элемента корзины с обновленными данными</param>
        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Удаляет элемент из корзины по ID
        /// </summary>
        /// <param name="cartItemId">ID элемента корзины</param>
        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var item = await GetCartItemAsync(cartItemId);
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Очищает корзину, удаляя все ее элементы
        /// </summary>
        /// <param name="cartId">ID корзины</param>
        public async Task ClearCartAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}
