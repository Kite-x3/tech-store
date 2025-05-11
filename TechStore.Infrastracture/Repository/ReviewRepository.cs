using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Domain.Interfaces;
using TechStore.Infrastracture.Data;
using TechStore.Infrastructure.Data;

namespace TechStore.Infrastructure.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Создаёт новый отзыв в бд
        /// </summary>
        /// <param name="review">Объект отзыва для создания</param>
        public async Task CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Удаляет отзыв по указанному ID
        /// </summary>
        /// <param name="id">ID отзыва для удаления</param>
        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Проверяет существование отзыва по ID
        /// </summary>
        /// <param name="id">ID отзыва</param>
        /// <returns>true если отзыв существует, иначе false</returns>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reviews.AnyAsync(r => r.ReviewId == id);
        }
        /// <summary>
        /// Получает отзыв по ID
        /// </summary>
        /// <param name="id">ID отзыва</param>
        /// <returns>Найденный отзыв или null</returns>
        public async Task<Review> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }
        /// <summary>
        /// Получает все отзывы для указанного товара, отсортированные по дате (новые сначала)
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns>Список отзывов</returns>
        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
        /// <summary>
        /// Обновляет информацию об отзыве в базе данных
        /// </summary>
        /// <param name="review">Обновленыый объект отзыва</param>
        public async Task UpdateReviewAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}