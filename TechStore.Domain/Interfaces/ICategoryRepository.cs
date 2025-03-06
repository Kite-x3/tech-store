using TechStore.Domain.Entities;

namespace TechStore.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesAsync ();
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync (int id);
        Task UpdateCategoryAsync(Category category);
    }
}
