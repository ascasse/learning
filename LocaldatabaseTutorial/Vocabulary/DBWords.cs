using Learning.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Learning  
{

    public class DBWords : IDisposable
    {
        private readonly string _database;
        private readonly Lazy<SQLiteAsyncConnection> _connection;
        private bool initialized = false;
        private bool disposedValue;

        public SQLiteAsyncConnection Connection => _connection.Value;

        public DBWords(string database)
        {
            _database = database;
            _connection = new Lazy<SQLiteAsyncConnection>(() => Initialize().Result, true);
            //InitializeAsync().SafeFireAndForget();
        }

        private async Task<SQLiteAsyncConnection> Initialize()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(_database, true);
            if (!initialized)
            {
                if (!connection.TableMappings.Any(m => m.MappedType.Name == typeof(Category).Name))
                {
                    await connection.CreateTablesAsync(CreateFlags.None, typeof(Category)).ConfigureAwait(false);
                }
                if (!connection.TableMappings.Any(m => m.MappedType.Name == typeof(Word).Name))
                {
                    await connection.CreateTablesAsync(CreateFlags.None, typeof(Word)).ConfigureAwait(false);
                }
                initialized = true;
            }
            return connection;
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                CheckDatabase();
                initialized = true;
            }
        }

        public async Task<int> CreateCategory(Category category)
        {
            var id = await Connection.InsertAsync(category);
            if (category.Words.Count > 0) {
                foreach (Word w in category.Words)
                {
                    w.CategoryId = category.Id;
                }
                await Connection.InsertAllAsync(category.Words);
            }
            return category.Id;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await Connection.Table<Category>().ToListAsync();
        }
        

        public async Task<Category> GetCategory(int id)
        {
            var category = await Connection.FindAsync<Category>(id);
            if (category == null)
                return null;
            var words = await Connection.QueryAsync<Word>("SELECT * FROM Words WHERE categoryId = ?", new object[] { category.Id });
            category.Words = words;
            return category;
        }

        public Task<Category> UpdateCategory(Category category)
        {
            category.Words.Where(x => x.Id == 0).ToList().ForEach(w => w.CategoryId = category.Id);
            
            Connection.RunInTransactionAsync((SQLiteConnection transaction) =>
            {
                Connection.InsertAllAsync(category.Words.Where(x => x.Id == 0).ToList());
                Connection.UpdateAllAsync(category.Words);
                Connection.UpdateAsync(category);
            });

            return GetCategory(category.Id);
        }

        public async Task<Category> UpdateCategoryUsage(int categoryId, DateTime datetime)
        {
            await Connection.ExecuteAsync("UPDATE Categories SET LastUse=? WHERE Id=?", new Object[] { datetime, categoryId });
            return await GetCategory(categoryId);
        }

        public Task<int> DeleteCategory(int id)
        {
            return Connection.DeleteAsync<Category>(id);
        }

        public Task<int> UpdateWords(List<Word>words)
        {
            return Connection.UpdateAllAsync(words);
        }

        private void CheckDatabase()
        {
            Connection.CreateTableAsync<Category>().Wait();
            Connection.CreateTableAsync<Word>().Wait();
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
                category.Words.Add(new Word() { Text = part });

            return category;
        }

        public Task<List<Category>> GetRecent(int days, int count)
        {
            string recent_day = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd");
            string sql = $"SELECT * FROM Categories WHERE lastUse > '{ recent_day }' OR lastUse = 0 ORDER BY lastUSE DESC LIMIT { count }";
            return Connection.QueryAsync<Category>(sql);
        }

        public async Task LoadFromCsvFile(string path)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                Category category = LoadFromString(line);
                await CreateCategory(category);
            }
        }

        //private IEnumerable<string> ImportCsv(string path)
        //{
        //    using (StreamReader sr = new StreamReader(path))
        //        while (!sr.EndOfStream)
        //            yield return sr.ReadLine();
        //}

        public Task<int> LoadCsv(string path)
        {
            List<Category> categories = new List<Category>();
            using (TextReader reader = File.OpenText(path))
            {
                CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Configuration.Delimiter = ";";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    Category category = csv.GetRecord<Category>();
                    categories.Add(category);
                }
            }
            return Connection.InsertAllAsync(categories);
        }

        public void DropTables()
        {
            Connection.DropTableAsync<Category>();
            Connection.DropTableAsync<Word>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_connection != null)
                        Connection.CloseAsync();
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DBWords()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
