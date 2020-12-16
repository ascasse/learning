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
            testCategory.Words.Select(w => w.LastUse = DateTime.Now.Date).ToList();
            testCategory.Words.Select(w => w.Views = 1).ToList();
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
            Assert.AreEqual(3, ctg.Words.Count);

            Assert.AreEqual("Pizza", testCategory.Words[0].Text);
            Assert.AreEqual("Ham", testCategory.Words[1].Text);
            Assert.AreEqual("Eggs", testCategory.Words[2].Text);
            foreach (Word w in ctg.Words)
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
            await db.Connection.DeleteAllAsync<Word>(); 

            Category category = GetTestCategory().Result;
            category.Words.RemoveAll(x => x.Text.Equals("Ham"));
            category.Words.Add(new Word() { Text = "Pasta" });

            Category updatedCategory = db.UpdateCategory(category).Result;
            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(3, updatedCategory.Words.Count);
        }

        [Test()]
        public async Task Update_Category_LastUse_Test()
        {
            await db.Connection.DeleteAllAsync<Word>(); 

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
            await db.Connection.DeleteAllAsync<Word>();

            Category category = GetTestCategory().Result;
            foreach (var word in category.Words)
            {
                word.Views += 1;
                word.LastUse = DateTime.Now.Date;
            }
            int updated = db.UpdateWords(category.Words).Result;

            Assert.AreEqual(3, updated);

            category = db.GetCategory(category.Id).Result;
            foreach (var word in category.Words)
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
            List<Word> words = new List<Word>() { new Word
            {
                Text = "Eggs",
                LastUse = DateTime.Today
            },
            new Word
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
            newCategory.Words = words;
            db.UpdateCategory(newCategory);
        }

        private Category LoadFromCsv(string csv)
        {
            var parts = csv.Split(",");
            Category category = new Category()
            {
                Name = parts[0],
                Words = new List<Word>()
            };
            foreach (var part in parts.Skip<string>(1))
            {
                category.Words.Add(new Word() { Text = part });
            }
            return category;
        }

        #endregion
    }
}