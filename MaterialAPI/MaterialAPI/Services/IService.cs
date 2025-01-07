using MaterialAPI.Model;

namespace MaterialAPI.Services
{
    public interface IService
    {
        void CheckDatabase();
        List<Category> GetRecent();
        List<Category> GetRecent(int batches = 5, int bits = 10, int days = 7);
        IEnumerable<Category> GetRecent2();
        Category BuildBatchFromCategory(int id);
        string GetFilePath(int id);
        string GetThumbnailPath(int id);
    }
}
