using TechStore.Domain.Entities;

namespace TechStore.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<CartItem> GetCartItemAsync(int cartItemId);
        Task AddCartAsync(Cart cart);
        Task AddCartItemAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
    }
}
