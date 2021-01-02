using Learning.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Learning
{
    public class Service
    {
        private DBWords Database { get; set; }
        /// <summary>Max number of recently viewed categories to return.</summary>
        public int RecentCount { get; set; } = 5;
        /// <summary>Date range to look for recently viewed categories or words, in days.</summary>
        public int RecentDays { get; set; } = 7;
        /// <summary>Max number of elements returned in a batch.</summary>
        public int BatchSize { get; set; } = 10;
        /// <summary>Max times a given element is included in a batch.</summary>
        public int MaxViews { get; set; } = 5;
        /// <summary>Number of elements replaced when a batch is updated.</summary>
        public int RefreshRate { get; set; } = 3;
        /// <summary>Set whether should update views counter for the same day.</summary>
        public bool SameDayCount { get; set; } = true;

        public Service(string connectionString)
        {
            Database = new DBWords(connectionString);
        }

        public async Task<List<Category>> GetCategories()
        {
            return await Database.GetCategories();
        }

        public async Task<Category> GetCategory(int id)
        {
            return await Database.GetCategory(id);
        }

        public async Task<int> CreateCategory(Category newCategory)
        {
            return await Database.CreateCategory(newCategory);
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            return await Database.UpdateCategory(category);
        }

        /// <summary>
        /// Updates view info of given words.
        /// </summary>
        /// A batch is just a subset of a category
        /// For each word, increase views by one and set lastUse date as today.
        /// Update category lastUse to today.
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task<Category> UpdateBatch(Category batch)
        {
            var currentDate = DateTime.Now;
            foreach (var word in batch.Words)
            {
                word.Views += 1;
                word.LastUse = currentDate;
            }
            batch.LastUse = currentDate;
            await Database.UpdateWords(batch.Words);
            await Database.UpdateCategoryUsage(batch.Id, currentDate);
            return batch;
        }

        public async Task<int> DeleteCategory(int categoryId)
        {
            return await Database.DeleteCategory(categoryId);
        }

        public async Task<List<Category>> GetRecent()
        {
            return await Database.GetRecent(RecentDays, RecentCount);
        }

        public async Task<Category> BuildBatchFromCategory(int category_id)
        {
            var category = await GetCategory(category_id);
            return BuildBatchFromCategory(category);
        }

        public Category BuildBatchFromCategory(Category category)
        {
            if (category.Words == null)
                return category;

            Category batch = new Category()
            {
                Id = category.Id,
                Name = category.Name
            };

            // Look for elements viewed fewer times than the max.
            var to_view = category.Words.Where(word => word.Views < MaxViews);

            // All elements have reached the max number of views. Return an empty batch
            if (to_view.Count() == 0)
                return batch;

            // Category has fewer or equal number of elements than batch size. 
            // The batch will be the category itself. Nothing to do.
            if (category.Words.Count <= BatchSize)
                return category;

            var sorted_words = category.Words.OrderByDescending(word => word.Views).ThenByDescending(word => word.LastUse);
            
            // If no element has reached max_views, take the first BatchSize elements
            if (sorted_words.First().Views < MaxViews)
            {
                batch.Words = sorted_words.Take(BatchSize).ToList();
                return batch;
            }

            if (to_view.Count() <= RefreshRate)
                batch.Words = sorted_words.Skip(Math.Max(0, sorted_words.Count() - BatchSize)).ToList();
            else
                batch.Words = sorted_words.Skip(Math.Max(0, sorted_words.Count() - to_view.Count() - (BatchSize - RefreshRate))).Take(BatchSize).ToList();

            return batch;
        }

        public async Task LoadFromFile(string path)
        {
            await Database.LoadFromCsvFile(path);
        }

        public async Task Close()
        {
            Database.DropTables();
            await Database.Connection.CloseAsync();
        }
    }
}
