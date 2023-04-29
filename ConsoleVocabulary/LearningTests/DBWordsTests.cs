using NUnit.Framework;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using Learning.Model;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Learning.Tests
{
    [TestFixture()]
    public class DBWordsTests
    {
        DBWords db;
        Category testCategory;

        [SetUp]
        public void SetUp()
        {
            db = new DBWords(":memory:");

            // Test category
            testCategory = db.LoadFromString("Food,Pizza,Ham,Eggs");
            testCategory.LastUse = DateTime.Now.Date;
            testCategory.Items.Select(w => w.LastUse = DateTime.Now.Date).ToList();
            testCategory.Items.Select(w => w.Views = 1).ToList();
        }


        [Test()]
        public void CheckDatabaseTest()
        {
            Assert.IsTrue(TableExists(db.Connection, "Categories").Result);
            Assert.IsTrue(TableExists(db.Connection, "Words").Result);
        }

        [Test()]
        public void Get_Category_Test()
        {
            Category ctg = GetTestCategory().Result;

            Assert.IsNotNull(ctg);
            Assert.AreEqual(1, ctg.Id);
            Assert.AreEqual(DateTime.Now.Date, ctg.LastUse);
            Assert.AreEqual(3, ctg.Items.Count);

            Assert.AreEqual("Pizza", testCategory.Items[0].Text);
            Assert.AreEqual("Ham", testCategory.Items[1].Text);
            Assert.AreEqual("Eggs", testCategory.Items[2].Text);
            foreach (Item w in ctg.Items)
            {
                Assert.AreEqual(DateTime.Now.Date, w.LastUse);
                Assert.AreEqual(1, w.Views);
            }
        }

        [Test()]
        public async Task Get_Category_Not_Found_Test()
        {
            var ctg = await db.GetCategory(33);
            Assert.IsNull(ctg);
        }

        //[Test()]
        //public void LoadFromCsvFileTest()
        //{
        //    db.LoadFromCsvFile(@".\TestData\categories.csv");
        //    List<Category> categories = db.Connection.Table<Category>().ToListAsync().Result;

        //    Assert.GreaterOrEqual(13, categories.Count);
        //}

        [Test()]
        public async Task Update_Category_Words_Test()
        {
            await db.Connection.DeleteAllAsync<Item>(); 

            Category category = GetTestCategory().Result;
            category.Items.RemoveAll(x => x.Text.Equals("Ham"));
            category.Items.Add(new Item() { Text = "Pasta" });

            Category updatedCategory = db.UpdateCategory(category).Result;
            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(3, updatedCategory.Items.Count);
        }

        [Test()]
        public async Task Update_Category_LastUse_Test()
        {
            await db.Connection.DeleteAllAsync<Item>(); 

            Category category = GetTestCategory().Result;
            Assert.IsNotNull(category);
            category.LastUse = DateTime.Today;

            Category updatedCategory = db.UpdateCategory(category).Result;

            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(DateTime.Today, updatedCategory.LastUse);
        }

        [Test()]
        public async Task Update_Words_View_Test()
        {
            await db.Connection.DeleteAllAsync<Item>();

            Category category = GetTestCategory().Result;
            foreach (var word in category.Items)
            {
                word.Views += 1;
                word.LastUse = DateTime.Now.Date;
            }
            int updated = db.UpdateWords(category.Items).Result;

            Assert.AreEqual(3, updated);

            category = db.GetCategory(category.Id).Result;
            foreach (var word in category.Items)
            {
                Assert.AreEqual(2, word.Views);
            }
        }

        #region Utility methods
        private async Task<Category> GetTestCategory()
        {
            int id = await db.CreateCategory(testCategory);
            Category ctg = await db.GetCategory(id);
            Debug.WriteLine($"Category id: { ctg.Id}");
            return ctg;
        }

        private async Task<Category> GetTestCategory(string name)
        {
            testCategory.Name = name;
            int id = await db.CreateCategory(testCategory);
            Category ctg = await db.GetCategory(id);
            Debug.WriteLine($"Category id: { ctg.Id}");
            return ctg;
        }


        private async Task<bool> TableExists(SQLiteAsyncConnection connection, string tableName)
        {
            //string query = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{ tableName }'";
            List<string> tables =
                await connection.QueryScalarsAsync<string>(
                    "SELECT name FROM sqlite_master WHERE type = 'table' AND name = ?", new object[] { tableName });
            return tables.Count > 0;
        }

        private void AddData()
        {
            List<Item> words = new List<Item>() { new Item
            {
                Text = "Eggs",
                LastUse = DateTime.Today
            },
            new Item
            {
                Text = "Pizza",
                LastUse = DateTime.Today
            }};

            var newCategory = new Category
            {
                Name = "Food",
                LastUse = DateTime.Today,
            };

            db.Connection.InsertAsync(newCategory);
            db.Connection.InsertAllAsync(words);
            newCategory.Items = words;
            db.UpdateCategory(newCategory);
        }

        private Category LoadFromCsv(string csv)
        {
            var parts = csv.Split(",");
            Category category = new Category()
            {
                Name = parts[0],
                Items = new List<Item>()
            };
            foreach (var part in parts.Skip<string>(1))
            {
                category.Items.Add(new Item() { Text = part });
            }
            return category;
        }

        #endregion
    }
}