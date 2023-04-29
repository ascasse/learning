using NUnit.Framework;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;
using Learning.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Learning.Tests
{
    [TestFixture()]
    public class ServiceTests
    {
        private Service service;

        [SetUp]
        public async Task SetUp()
        {
            service = new Service(":memory:");
            //service = new Service(@".\TestData\testdb.db3");
            service.BatchSize = 5;
            service.MaxViews = 2;
            service.RefreshRate = 3;
            await service.LoadFromFile(@".\TestData\categories.csv");
        }

        [Test()]
        public async Task UpdateBatchTest()
        {
            Category batch = new Category()
            {
                Name = "Test",
                LastUse = DateTime.Now.AddDays(-3),
                Items = new List<Item>() {
                    new Item() { Text = "word1", LastUse = DateTime.Now.AddDays(-3), Views = 1 },
                    new Item() { Text = "word2", LastUse = DateTime.Now.AddDays(-3), Views = 1 },
                    new Item() { Text = "word3", LastUse = DateTime.MinValue, Views = 0 },
                    new Item() { Text = "word4", LastUse = DateTime.MinValue, Views = 0 },
                    new Item() { Text = "word5", LastUse = DateTime.MinValue, Views = 0 }
                }
            };

            Category updatedBatch = await service.UpdateBatch(batch);

            Assert.IsTrue(updatedBatch.LastUse.Date == DateTime.Now.Date);
            foreach (Item w in updatedBatch.Items)
            {
                Assert.IsTrue(w.LastUse.Date == DateTime.Now.Date);
                if (w.Text.Equals("word1") || w.Text.Equals("word2"))
                    Assert.IsTrue(w.Views == 2);
                else
                    Assert.IsTrue(w.Views == 1);
            }
        }

        [Test()]
        public void GetRecentTest()
        {
            List<Category> recent = service.GetRecent().Result;
            Assert.True(service.RecentCount >= recent.Count);
        }

        [Test()]
        public async Task BuildBatchFromCategoryTest()
        {
            Category ctg = service.GetCategory(2).Result;
            Assert.IsNotNull(ctg);
            Assert.IsNotNull(ctg.Items);
            Assert.IsNotEmpty(ctg.Items);

            Category batch1 = await service.BuildBatchFromCategory(ctg.Id);
            Assert.IsNotNull(batch1);
            Assert.IsNotNull(batch1.Items);
            Assert.AreEqual(service.BatchSize, batch1.Items.Count);

            batch1 = await service.UpdateBatch(batch1);
            var views = batch1.Items.Select(w => w.Views);
            Assert.AreEqual(1, views.ElementAt(0));
            Assert.False(views.Any(o => o != views.ElementAt(0)));

            // Second pass
            Category batch2 = await service.BuildBatchFromCategory(ctg.Id);
            Assert.IsNotNull(batch2);
            Assert.AreEqual(service.BatchSize, batch2.Items.Count);

            // Both batches should have the same words.
            var words1 = batch1.Items.Select(w => w.Id);
            var words2 = batch2.Items.Select(w => w.Id);
            Assert.IsFalse(words1.Except(words2).Any());

            batch2 = await service.UpdateBatch(batch2);
            views = batch2.Items.Select(w => w.Views);
            Assert.AreEqual(2, views.ElementAt(0));
            Assert.False(views.Any(o => o != views.ElementAt(0)));

            // Third pass. Reached MaxViews
            Category batch3 = await service.BuildBatchFromCategory(ctg.Id);
            Assert.IsNotNull(batch3);
            Assert.AreEqual(service.BatchSize, batch3.Items.Count);

            // There should be RefreshRate new words.
            var words3 = batch3.Items.Select(w => w.Id);
            Assert.AreEqual(service.RefreshRate, words2.Except(words3).Count());
        }

        [Test()]
        public async Task All_elements_viewed_at_least_MaxViews_times()
        {
            Category ctg = service.GetCategory(1).Result;

            Category batch = service.BuildBatchFromCategory(ctg);
            while (batch.Items != null)
            {
                var words = batch.Items.Select(w => w.Id);
                TestContext.WriteLine(string.Join(",", words));
                await service.UpdateBatch(batch);
                batch = service.BuildBatchFromCategory(ctg);
            }
            TestContext.WriteLine($"Completed");
            ctg = service.GetCategory(1).Result;
            var views = ctg.Items.Select(w => w.Views);
            TestContext.WriteLine($"Views: { string.Join(",", views) }.");

            Assert.IsFalse(views.Where(v => v < service.MaxViews).Any());
        }
    }
}