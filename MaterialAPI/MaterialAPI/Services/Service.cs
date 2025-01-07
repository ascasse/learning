using MaterialAPI.Data;
using MaterialAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace MaterialAPI.Services
{
    public class Service : IService
    {
        private readonly MaterialAPIContext db;

        private static int recentDays = 7;
        private static int batchSize = 10;
        private static int batchCount = 5;

        /// <summary>Max number of recently viewed categories to return.</summarbatchCount >
        public int RecentCount { get; set; } = batchCount;
        /// <summary>Date range to look for recently viewed categories or words, in daysrecentDays</summary>
        public int RecentDays { get; set; } = recentDays;
        /// <summary>Max number of elements returned in a batch.</summary>
        public int BatchSize { get; set; } = batchSize;
        /// <summary>Max times a given element is included in a batch.</summary>
        public int MaxViews { get; set; } = 5;
        /// <summary>Number of elements replaced when a batch is updated.</summary>
        public int RefreshRate { get; set; } = 3;
        /// <summary>Set whether should update views counter for the same day.</summary>
        public bool SameDayCount { get; set; } = true;

        public Service(MaterialAPIContext db) 
        {
            this.db = db;
        }

        public void CheckDatabase()
        {
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT name from sqlite_master WHERE type='table'";
                db.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        Console.WriteLine(result.GetString(0));
                    }
                }
            }
        }


        public IEnumerable<Category> GetRecent2()
        {
            DateTime recent_day = DateTime.Now.AddDays(-RecentDays);
            return db.Categories.Include("Items").Where(c => c.LastUse > recent_day || c.LastUse == DateTime.MinValue).OrderByDescending(c => c.LastUse).Take(RecentCount).AsEnumerable();
        }

        public List<Category> GetRecent()
        {
            DateTime recent_day = DateTime.Now.AddDays(-RecentDays);
            IEnumerable<Category> categories = db.Categories.Include("Items")
                .Where(c => c.LastUse > recent_day || 
                    c.LastUse == DateTime.MinValue ||
                    c.LastUse == null)
                .OrderByDescending(c => c.LastUse).Take(RecentCount).AsEnumerable();
            List<Category> recent = new();
            foreach (var category in categories)
                recent.Add(BuildBatchFromCategory(category));

            return recent;
        }

        public List<Category> GetRecent(int batches = 5, int bits = 10, int days = 7)
        {
            DateTime recent_day = DateTime.Now.AddDays(-days);
            IEnumerable<Category> categories = db.Categories.Include("Items")
                .Where(c => c.LastUse > recent_day ||
                    c.LastUse == DateTime.MinValue ||
                    c.LastUse == null)
                .OrderByDescending(c => c.LastUse).Take(batches).AsEnumerable();
            List<Category> recent = new();
            foreach (var category in categories)
                recent.Add(BuildBatchFromCategory(category, bits));

            return recent;
        }

        public Category BuildBatchFromCategory(int id)
        {
            var category = db.Categories.Include("Items").FirstOrDefault(c => c.Id == id);
            if (category == null) { return new Category(); }

            return BuildBatchFromCategory(category);
        }

        public Category BuildBatchFromCategory(Category category, int size = 10)
        {
            if (category.Items == null)
                return category;

            Category batch = new Category(category);

            // Look for elements viewed fewer times than the max.
            var to_view = category.Items.Where(word => word.Views < MaxViews);

            // All elements have reached the max number of views. Return an empty batch
            if (to_view.Count() == 0)
                return batch;

            // Category has fewer or equal number of elements than batch size. 
            // The batch will be the category itself. Nothing to do.
            if (category.Items.Count <= size)
                return category;

            var sorted_words = category.Items.OrderByDescending(word => word.Views).ThenByDescending(word => word.LastUse);

            // If no element has reached max_views, take the first BatchSize elements
            if (sorted_words.First().Views < MaxViews)
            {
                batch.Items = sorted_words.Take(size).ToList();
                return batch;
            }

            if (to_view.Count() <= RefreshRate)
                batch.Items = sorted_words.Skip(Math.Max(0, sorted_words.Count() - size)).ToList();
            else
                batch.Items = sorted_words.Skip(Math.Max(0, sorted_words.Count() - to_view.Count() - (size - RefreshRate))).Take(size).ToList();

            return batch;
        }

        public int UpdateBatch(Category batch)
        {
            Category toUpdate = db.Categories.Include("Items").FirstOrDefault(c => c.Id == batch.Id) ?? throw new ArgumentException("Category not found.");

            DateTime now = DateTime.Now;
            foreach (Item item in toUpdate.Items
                .Where(item => batch.Items.Select(item => item.Id).Contains(item.Id))) 
            {
                item.Views += 1;
                item.LastUse = now;
            }
            toUpdate.LastUse = now;

            return db.SaveChanges();
        }

        public int ResetCategory(int id)
        {
            Category toUpdate = db.Categories.Include("Items").FirstOrDefault(c => c.Id == id) ?? throw new ArgumentException("Category not found.");
            foreach (Item item in toUpdate.Items)
            {
                item.Views = 0;
                item.LastUse = DateTime.MinValue;
            }
            toUpdate.LastUse = DateTime.MinValue;

            return db.SaveChanges();
        }

        public string GetFilePath(int id)
        {
            string basePath = @"e:\material\bits";

            var item = db.Items.Find(id);
            if (item == null || item.Image == null) return string.Empty;
            return Path.Combine(basePath, item.Image);
            //return "c:\\background\\1920x1080\\tren.jpg";
            //return item != null ? item.Image : string.Empty;
        }

        public String GetThumbnailPath(int id)
        {
            string imagePath = GetFilePath(id);
            return imagePath.Replace("bits", @"thumbnails");
        }
    }
}
