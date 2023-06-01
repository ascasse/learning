using MaterialAPI.Model;

namespace MaterialAPI.Services
{
    public interface IService
    {
        IEnumerable<Category> GetRecent();
    }
}
