using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Получает отфильтрованный список товаров с пагинацией и сортировкой
        /// </summary>
        /// <param name="categoryId">ID категории для фильтрации (опционально)</param>
        /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        /// <param name="sortBy">Поле для сортировки (price/name/date)</param>
        /// <param name="sortDescending">Направление сортировки (по убыванию)</param>
        /// <param name="minPrice">Минимальная цена для фильтрации (опционально)</param>
        /// <param name="maxPrice">Максимальная цена для фильтрации (опционально)</param>
        /// <returns>Кортеж (список товаров, общее количество)</returns>
        public async Task<(IEnumerable<Product> products, int totalCount)> GetFilteredProductsAsync(
            int? categoryId,
            int pageNumber,
            int pageSize,
            string sortBy,
            bool sortDescending,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            // Фильтрация
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Сортировка
            query = sortBy?.ToLower() switch
            {
                "price" => sortDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                "name" => sortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "date" => sortDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.ProductId)
            };

            // Пагинация
            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
        /// <summary>
        /// Создает новый товар в базе данных
        /// </summary>
        /// <param name="product">Объект товара для создания</param>
        public async Task CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Удаляет товар по указанному ID
        /// </summary>
        /// <param name="id">ID товара для удаления</param>
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// получение товара по его id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Найденный товар или null</returns>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        /// <summary>
        /// получение нескольких случайных товаров для главной страницы
        /// </summary>
        /// <returns>Список случайных товаров</returns>
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var random = new Random();
            var count = await _context.Products.CountAsync();
            var products = new List<Product>();

            for (int i = 0; i < 10; i++)
            {
                var skip = random.Next(0, count);
                var product = await _context.Products
                    .Skip(skip)
                    .FirstOrDefaultAsync();

                if (product != null)
                    products.Add(product);
            }

            return products;
        }
        /// <summary>
        /// Обновляет информацию о товаре в базе данных
        /// </summary>
        /// <param name="product">Объект товара с обновленными данными</param>
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}
