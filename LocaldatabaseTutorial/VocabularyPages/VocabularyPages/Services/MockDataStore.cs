using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocabularyPages.Models;

namespace VocabularyPages.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;
        readonly List<Category> recent;

        public MockDataStore()
        {
            items = new List<Item>()
            {
                new Item { Id = 1, Text = "First item", Description="This is an item description." },
                new Item { Id = 2, Text = "Second item", Description="This is an item description." },
                new Item { Id = 3, Text = "Third item", Description="This is an item description." },
                new Item { Id = 4, Text = "Fourth item", Description="This is an item description." },
                new Item { Id = 5, Text = "Fifth item", Description="This is an item description." },
                new Item { Id = 6, Text = "Sixth item", Description="This is an item description." }
            };

            recent = new List<Category>()
            {
                new Category { Id = 1, Name = "Category 1", LastUse = DateTime.Now,
                    Words = new List<Word>
                    {
                        new Word { Id = 1, Text = "Word 1", LastUse = DateTime.Now, Views = 1 },
                        new Word { Id = 2, Text = "Word 2", LastUse = DateTime.Now, Views = 1 },
                    }
                },
                new Category { Id = 2, Name = "Category 2", LastUse = DateTime.Now,
                    Words = new List<Word>
                    {
                        new Word { Id = 3, Text = "Word 3", LastUse = DateTime.Now, Views = 1 },
                        new Word { Id = 4, Text = "Word 4", LastUse = DateTime.Now, Views = 1 },
                    }
                },
            };
        }


        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(int id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(recent);
        }
    }
}