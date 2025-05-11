using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastructure.Repository
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly AppDbContext _context;

        public OrderStatusRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Получает все статусы заказов
        /// </summary>
        /// <returns>Список всех статусов заказов</returns>
        public async Task<IEnumerable<OrderStatus>> GetAllStatusesAsync()
        {
            return await _context.OrderStatuses.ToListAsync();
        }
        /// <summary>
        /// Получает статус заказа по ID
        /// </summary>
        /// <param name="id">ID статуса заказа</param>
        /// <returns>Найденный статус заказа</returns>
        public async Task<OrderStatus> GetStatusByIdAsync(int id)
        {
            return await _context.OrderStatuses.FindAsync(id);
        }
        /// <summary>
        /// Получает статус заказа по названию
        /// </summary>
        /// <param name="name">Название статуса</param>
        /// <returns>Найденный статус заказа</returns>
        public async Task<OrderStatus> GetStatusByNameAsync(string name)
        {
            return await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == name);
        }
        /// <summary>
        /// Проверяет существование статуса заказа по ID
        /// </summary>
        /// <param name="id">ID статуса заказа</param>
        /// <returns>True если статус существует, иначе False</returns>
        public async Task<bool> StatusExistsAsync(int id)
        {
            return await _context.OrderStatuses
                .AnyAsync(s => s.OrderStatusId == id);
        }
    }
}