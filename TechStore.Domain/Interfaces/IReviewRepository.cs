using TechStore.Domain.Entities;

namespace TechStore.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetReviewByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task CreateReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}