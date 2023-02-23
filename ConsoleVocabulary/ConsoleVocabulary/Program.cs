using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Learning;
using Learning.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;

namespace Vocabulary
{
    class Program
    {
        public static async Task Main()
        {
            //Service service = new Service("Vocabulary.db3")
            //{
            //    BatchSize = 5,
            //    MaxViews = 2,
            //    RefreshRate = 3
            //};

            //Category ctg = await service.GetCategory(1);
            //Console.WriteLine(ctg.Name);

            ManageBatches();

            Console.ReadLine();
        }

        private static async void ManageBatches()
        {
            Service service = new Service(":memory:")
            {
                BatchSize = 5,
                MaxViews = 2,
                RefreshRate = 3
            };

            await service.LoadFromFile(@".\Data\categories.csv");

            Category ctg = service.GetCategory(1).Result;
            Console.WriteLine($"Category {ctg.Name}.");
            Console.WriteLine($"{ctg.Words.Count} words in category.");
            foreach (Word w in ctg.Words)
            {
                Console.WriteLine(w.Text);
            }
            int iteration = 0;
            while (iteration < 10)
            {
                Category batch = service.BuildBatchFromCategory(ctg);
                if (batch.Words == null)
                    break;
                var words = batch.Words.Select(w => w.Id);
                Console.WriteLine($"Iteration { ++iteration }");
                Console.WriteLine(string.Join(",", words));
                await service.UpdateBatch(batch);
            }
            Console.WriteLine($"Completed");
            ctg = service.GetCategory(1).Result;
            var views = ctg.Words.Select(w => w.Views);
            Console.WriteLine(string.Join(",", views));
            await service.Close();
        }
    }
}
