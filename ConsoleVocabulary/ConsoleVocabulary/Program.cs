using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Learning;
using Learning.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Vocabulary
{
    class Program
    {
        static void Main(string[] args)
        {

            Service service = new Service(":memory:");
            service.BatchSize = 5;
            service.MaxViews = 2;
            service.RefreshRate = 3;
            service.LoadFromFile(@".\Data\categories.csv");

            Category ctg = service.GetFullCategory(1);

            int iteration = 0;
            while (iteration < 10)
            {
                Category batch = service.BuildBatchFromCategory(ctg);
                if (batch.Words == null)
                    break;
                var words = batch.Words.Select(w => w.Id);
                Console.WriteLine($"Iteration { ++iteration }");
                Console.WriteLine(string.Join(",", words));
                service.UpdateBatch(batch);
            }
            Console.WriteLine($"Completed");
            ctg = service.GetFullCategory(1);
            var views = ctg.Words.Select(w => w.Views);
            Console.WriteLine(string.Join(",", views));



            //Console.WriteLine("Creating database, if it doesn't already exist");
            //string dbPath = Path.Combine(
            //     Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            //     "Vocabulary.db3");

            //var db = new SQLiteConnection(dbPath);

            //db.DropTable<Category>();
            //db.DropTable<Word>();
            //db.CreateTable<Category>();
            //db.CreateTable<Word>();

            //List<Word> words = new List<Word>() { new Word
            //{
            //    Text = "Eggs",
            //    LastUse = DateTime.Today
            //},
            //new Word
            //{
            //    Text = "Pizza",
            //    LastUse = DateTime.Today
            //}};

            //var newCategory = new Category
            //{
            //    Name = "Food",
            //    LastUse = DateTime.Today,
            //};

            //db.Insert(newCategory);
            //db.InsertAll(words);
            //newCategory.Words = words;
            //db.UpdateWithChildren(newCategory);

            //Console.WriteLine("Reading data");
            //var category = db.GetWithChildren<Category>(newCategory.Id);
            //Console.WriteLine($"Category {category.Id}, {category.Name}");
            //foreach (Word word in category.Words)
            //    Console.WriteLine($"Word { word.Id }, {word.Text }, {word.LastUse }");

            Console.ReadLine();
        }
    }
}
