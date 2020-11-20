using Learning.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Learning  
{

    public class DBWords
    {
        private readonly string _database;
        private SQLiteConnection _connection;
        public SQLiteConnection Connection => _connection ?? OpenConnection();

        public DBWords(string database)
        {
            this._database = database;
            CheckDatabase();
        }

        private SQLiteConnection OpenConnection()
        {
            _connection = new SQLiteConnection(_database, true);
            return _connection;
        }

        public int CreateCategory(Category category)
        {
            Connection.Insert(category);
            if (category.Words.Count > 0) {
                Connection.InsertAll(category.Words);
                Connection.UpdateWithChildren(category);
            }
            return category.Id;
        }

        public List<Category> GetCategories()
        {
            return Connection.Table<Category>().ToList();
        }
        
        /// <summary>Find the category with the given id.</summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Category GetCategory(int id)
        {
            try
            {
                return Connection.Get<Category>(id);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Category GetFullCategory(int id)
        {
            try
            {
                return Connection.GetWithChildren<Category>(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Category UpdateCategory(Category category)
        {
            // Insert all the words new to the category
            Connection.InsertAll(category.Words.Where(x => x.Id == 0).ToList());
            Connection.UpdateWithChildren(category);
            return GetFullCategory(category.Id);
        }

        public Category UpdateCategoryUsage(int categoryId, DateTime datetime)
        {
            Connection.Execute("UPDATE Categories SET LastUse=? WHERE Id=?", datetime, categoryId);
            return GetCategory(categoryId);
        }

        public void DeleteCategory(int id)
        {
            Connection.Delete<Category>(id);
        }

        public void UpdateWords(List<Word>words)
        {
            Connection.UpdateAll(words);
        }

        private void CheckDatabase()
        {
            Connection.CreateTable<Category>();
            Connection.CreateTable<Word>();
        }

        public Category LoadFromString(string csv)
        {
            var parts = csv.Split(',');
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

        public void LoadFromCsvFile(string path)
        {
            foreach (string line in ImportCsv(path))
            {
                Category category = LoadFromString(line);
                CreateCategory(category);
            }
        }

        private IEnumerable<string> ImportCsv(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    yield return sr.ReadLine();
                }
            }
        }
    }
}
