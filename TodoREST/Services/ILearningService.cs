using TodoREST.Models;

namespace TodoREST.Services
{
    public interface ILearningService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task SaveCategoryAsync(Category item, bool isNewItem);
        Task DeleteCategoryAsync(Category item);
    }
}
