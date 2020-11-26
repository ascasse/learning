using NUnit.Framework;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Linq;
using Learning.Model;
using SQLiteNetExtensions.Extensions;

namespace Learning.Tests
{
    [TestFixture()]
    public class DBWordsTests
    {
        DBWords db;

        [SetUp]
        public void SetUpBeforeTest()
        {
            db = new DBWords(":memory:");
        }

        [Test()]
        public void CheckDatabaseTest()
        {
            Assert.IsTrue(TableExists(db.Connection, "Categories"));
            Assert.IsTrue(TableExists(db.Connection, "Words"));
        }

        [Test()]
        public void Get_Category_Test()
        {
            db.CreateCategory(db.LoadFromString("Food,Pizza,Ham,Eggs"));

            Category category = db.GetCategory(1);
            Assert.IsNotNull(category);
            Assert.AreEqual("Food", category.Name);
        }

        [Test()]
        public void Get_Category_Not_Found_Test()
        {
            db.CreateCategory(db.LoadFromString("Food,Pizza,Ham,Eggs"));

            Category category = db.GetCategory(33);
            Assert.IsNull(category);
        }

        [Test()]
        public void LoadFromCsvFileTest()
        {
            db.LoadFromCsvFile(@".\TestData\categories.csv");
            List<Category> categories = db.Connection.GetAllWithChildren<Category>();

            Assert.IsTrue(categories.Count == 13);
        }

        [Test()]
        public void Update_Category_Words_Test()
        {
            db.CreateCategory(db.LoadFromString("Food,Pizza,Ham,Eggs"));

            Category category = db.GetFullCategory(1);
            category.Words.Remove(category.Words.Single<Word>(x => x.Text.Equals("Ham")));
            category.Words.Add(new Word() { Text = "Pasta" });

            Category updatedCategory = db.UpdateCategory(category);

            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(3, updatedCategory.Words.Count);
        }

        [Test()]
        public void Update_Category_Test()
        {
            db.CreateCategory(db.LoadFromString("Food,Pizza,Ham,Eggs"));

            Category category = db.GetFullCategory(1);
            category.LastUse = DateTime.Today;

            Category updatedCategory = db.UpdateCategory(category);

            Assert.IsNotNull(updatedCategory);
            Assert.AreEqual(DateTime.Today, updatedCategory.LastUse);
        }


        private bool TableExists(SQLiteConnection connection, string tableName)
        {
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = string.Format("SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{0}'", tableName)
            };
            List<string> tables = command.ExecuteQueryScalars<string>().ToList();
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

            db.Connection.Insert(newCategory);
            db.Connection.InsertAll(words);
            newCategory.Words = words;
            db.Connection.UpdateWithChildren(newCategory);
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

        [Test()]
        public void Update_Words_View_Test()
        {
            db.CreateCategory(db.LoadFromString("Food,Pizza,Ham,Eggs"));
            Category category = db.GetFullCategory(1);
            foreach (var word in category.Words)
            {
                word.Views += 1;
                word.LastUse = DateTime.Now;
            }
            db.UpdateWords(category.Words);

            category = db.GetFullCategory(1);
            foreach (var word in category.Words)
            {
                Assert.AreEqual(1, word.Views);
            }
        }
    }
}