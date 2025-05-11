using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Проверяет существование заказа по ID
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <returns>True если заказ существует, иначе False</returns>
        public async Task<bool> ExistsAsync(int orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }
        /// <summary>
        /// Создаёт новый заказ в базе данных
        /// </summary>
        /// <param name="order">Объект заказа для создания</param>
        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Получает заказ по ID со связанными данными (статус, товары, пользователь)
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Найденный заказ</returns>
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }
        /// <summary>
        /// Получает все заказы со связанными данными (статус, товары, пользователь)
        /// </summary>
        /// <returns>Список всех заказов</returns>
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToListAsync();
        }
        /// <summary>
        /// Получает все заказы указанного пользователя со связанными данными (статус, товары, пользователь)
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список заказов пользователя</returns>
        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
        /// <summary>
        /// Обновляет информацию о заказе в базе данных
        /// </summary>
        /// <param name="order">Объект заказа с обновленными данными</param>
        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}