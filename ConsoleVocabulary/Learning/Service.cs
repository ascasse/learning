using Learning.Model;
using System;
using System.Collections.Generic;
using System.Linq;


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

        public List<Category> GetCategories()
        {
            return Database.GetCategories();
        }

        public Category GetCategory(int id)
        {
            return Database.GetCategory(id);
        }

        public Category GetFullCategory(int id)
        {
            return Database.GetFullCategory(id);
        }

        public int CreateCategory(Category newCategory)
        {
            return Database.CreateCategory(newCategory);
        }

        public Category UpdateCategory(Category category)
        {
            return Database.UpdateCategory(category);
        }

        /// <summary>
        /// Updates view info of given words.
        /// </summary>
        /// A batch is just a subset of a category
        /// For each word, increase views by one and set lastUse date as today.
        /// Update category lastUse to today.
        /// <param name="category"></param>
        /// <returns></returns>
        public Category UpdateBatch(Category batch)
        {
            var currentDate = DateTime.Now;
            foreach (var word in batch.Words)
            {
                word.Views += 1;
                word.LastUse = currentDate;
            }
            batch.LastUse = currentDate;
            Database.UpdateWords(batch.Words);
            Database.UpdateCategoryUsage(batch.Id, currentDate);
            return batch;
        }

        public int DeleteCategory(int categoryId)
        {
            return Database.DeleteCategory(categoryId);
        }

        public List<Category> GetRecent()
        {
            return Database.GetRecent(RecentDays, RecentCount);
        }

        public Category BuildBatchFromCategory(int category_id)
        {
            Category category = GetCategory(category_id);
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

        public void LoadFromFile(string path)
        {
            Database.LoadFromCsvFile(path);
        }
    }
}
