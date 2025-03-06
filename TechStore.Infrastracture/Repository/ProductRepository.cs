using Microsoft.EntityFrameworkCore;
using System.Linq;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastracture.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, string? name)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.Category);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.Category.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            return await query.ToListAsync();
        }



        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = _context.Products.FindAsync(product.ProductId);
            if (existingProduct != null)
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
