using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VocabularyPages.Services
{
    public interface IVocabularyData<T>
    {
        Task<int> AddItemAsync(T item);
        Task<T> UpdateItemAsync(T item);
        Task<int> DeleteItemAsync(int id);
        Task<T> GetItemAsync(int id);
        Task<IEnumerable<T>> GetItemsAsync();
        Task<IEnumerable<T>> GetRecentAsync(bool forceRefresh = false);
    }
}
