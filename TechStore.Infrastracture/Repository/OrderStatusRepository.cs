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

        public async Task<IEnumerable<OrderStatus>> GetAllStatusesAsync()
        {
            return await _context.OrderStatuses.ToListAsync();
        }

        public async Task<OrderStatus> GetStatusByIdAsync(int id)
        {
            return await _context.OrderStatuses.FindAsync(id);
        }

        public async Task<OrderStatus> GetStatusByNameAsync(string name)
        {
            return await _context.OrderStatuses
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<bool> StatusExistsAsync(int id)
        {
            return await _context.OrderStatuses
                .AnyAsync(s => s.OrderStatusId == id);
        }
    }
}