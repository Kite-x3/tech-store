using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastracture.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Проверяет существование категории по ID
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>True если категория существует, иначе False</returns>
        public async Task<bool> ExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }
        /// <summary>
        /// Создаёт новую категорию в базе данных
        /// </summary>
        /// <param name="category">Объект категории для создания</param>
        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Удаляет категорию по указанному ID
        /// </summary>
        /// <param name="id">ID категории для удаления</param>
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
        // <summary>
        /// Получает список всех категорий
        /// </summary>
        /// <returns>Список категорий</returns>
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        /// <summary>
        /// Получает категорию по ID со связанными товарами
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <returns>Найденная категория или null</returns>
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }
        /// <summary>
        /// Обновляет информацию о категории в базе данных
        /// </summary>
        /// <param name="category">Объект категории с обновленными данными</param>
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
