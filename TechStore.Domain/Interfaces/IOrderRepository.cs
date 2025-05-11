using TechStore.Domain.Entities;

namespace TechStore.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task<bool> ExistsAsync(int orderId);
    }
}