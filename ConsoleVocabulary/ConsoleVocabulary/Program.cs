﻿using System;
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
            //ManageDB();

            Console.ReadLine();
        }

        private static async void ManageBatches()
        {
            //Service service = new Service(":memory:")
            Service service = new Service(@".\Data\material.db3")
            {
                BatchSize = 5,
                MaxViews = 3,
                RefreshRate = 3
            };

            await service.LoadFromFile(@".\Data\categories.csv");

            Category ctg = service.GetCategory(1).Result;
            Console.WriteLine($"Category {ctg.Name}.");
            Console.WriteLine($"{ctg.Items.Count} words in category.");
            foreach (Item w in ctg.Items)
            {
                Console.WriteLine(w.Text);
            }
            int iteration = 0;
            while (iteration < 10)
            {
                Category batch = service.BuildBatchFromCategory(ctg);
                if (batch.Items == null)
                    break;
                var words = batch.Items.Select(w => w.Text);
                Console.WriteLine($"Iteration { ++iteration }");
                Console.WriteLine(string.Join(",", words));
                await service.UpdateBatch(batch);
            }
            Console.WriteLine($"Completed");
            ctg = service.GetCategory(1).Result;
            var views = ctg.Items.Select(w => $"{w.Text}: {w.Views}");
            foreach (string view in views)
                Console.WriteLine(view);
            await service.Close();
        }

        private static async void ManageDB()
        {
            Service service = new Service(@".\Data\material.db3")
            {
                BatchSize = 5,
                MaxViews = 3,
                RefreshRate = 3
            };

            Category ctg = service.GetCategory(1)?.Result;
            if (ctg == null)
            {
                Console.WriteLine("Not found.");
                return;
            }
                
            Console.WriteLine($"Category {ctg.Name}.");
            Console.WriteLine($"{ctg.Items.Count} words in category.");
            foreach (Item w in ctg.Items)
            {
                Console.WriteLine(w.Text);
            }
            int iteration = 0;
            while (iteration < 10)
            {
                Category batch = service.BuildBatchFromCategory(ctg);
                if (batch.Items == null)
                    break;
                var words = batch.Items.Select(w => w.Text);
                Console.WriteLine($"Iteration {++iteration}");
                Console.WriteLine(string.Join(",", words));
                await service.UpdateBatch(batch);
            }
            Console.WriteLine($"Completed");
            ctg = service.GetCategory(1).Result;
            var views = ctg.Items.Select(w => $"{w.Text}: {w.Views}");
            foreach (string view in views)
                Console.WriteLine(view);
            await service.Close();
        }
    }
}
