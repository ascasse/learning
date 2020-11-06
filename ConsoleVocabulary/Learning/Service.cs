using Learning.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learning
{
    class Service
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
        /// For each word increases views by one and set lastUse date as today.
        /// Updates category lastUse to today.
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
            


            return null;

        }




    }
}
