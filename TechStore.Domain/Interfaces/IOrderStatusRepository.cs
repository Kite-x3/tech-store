using TechStore.Domain.Entities;

namespace TechStore.Domain.Interfaces
{
    public interface IOrderStatusRepository
    {
        Task<OrderStatus> GetStatusByIdAsync(int id);
        Task<OrderStatus> GetStatusByNameAsync(string name);
        Task<bool> StatusExistsAsync(int id);
        Task<IEnumerable<OrderStatus>> GetAllStatusesAsync();
    }
}