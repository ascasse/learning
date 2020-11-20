using System;
using System.Collections.Generic;
using System.IO;
using Learning.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Vocabulary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating database, if it doesn't already exist");
            string dbPath = Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                 "Vocabulary.db3");

            var db = new SQLiteConnection(dbPath);

            db.DropTable<Category>();
            db.DropTable<Word>();
            db.CreateTable<Category>();
            db.CreateTable<Word>();

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

            db.Insert(newCategory);
            db.InsertAll(words);
            newCategory.Words = words;
            db.UpdateWithChildren(newCategory);

            Console.WriteLine("Reading data");
            var category = db.GetWithChildren<Category>(newCategory.Id);
            Console.WriteLine(string.Format("Category {0}, {1}", category.Id, category.Name));
            foreach (Word word in category.Words)
            {
                Console.WriteLine(string.Format("Word {0}, {1}, {2}", word.Id, word.Text, word.LastUse));
            }

            Console.ReadKey();
        }
    }
}
