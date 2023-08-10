using MaterialAPI.Model;

namespace MaterialAPI.Services
{
    public interface IService
    {
        void CheckDatabase();
        List<Category> GetRecent();
        IEnumerable<Category> GetRecent2();
        Category BuildBatchFromCategory(int id);
    }
}
