using TodoREST.Models;

namespace TodoREST.Services
{
    public class LearningService : ILearningService
    {
        readonly ILearningRestService iLearningRestService;

        public LearningService(ILearningRestService iLearningService)
        {
            iLearningRestService = iLearningService;
        }

        public Task DeleteCategoryAsync(Category category)
        {
            return iLearningRestService.DeleteCategoryAsync(category.Id);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return iLearningRestService.GetCategoriesAsync();
        }

        public Task SaveCategoryAsync(Category category, bool isNewItem)
        {
            return iLearningRestService.SaveCategoryAsync(category, isNewItem);
        }
    }
}
