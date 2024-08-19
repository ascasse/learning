using TodoREST.Models;

namespace TodoREST.Services
{
    public interface ILearningRestService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task SaveCategoryAsync(Category item, bool isNewItem);
        Task DeleteCategoryAsync(int id);
    }
}
