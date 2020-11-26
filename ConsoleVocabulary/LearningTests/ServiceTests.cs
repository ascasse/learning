using NUnit.Framework;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;
using Learning.Model;
using System.Linq;

namespace Learning.Tests
{
    [TestFixture()]
    public class ServiceTests
    {
        private Service service;

        [SetUp]
        public void SetUp()
        {
            service = new Service(":memory:");
            //service = new Service(@".\TestData\testdb.db3");
            service.BatchSize = 5;
            service.MaxViews = 2;
            service.RefreshRate = 3;
            service.LoadFromFile(@".\TestData\categories.csv");
        }

        [Test()]
        public void UpdateBatchTest()
        {
            Category batch = new Category()
            {
                Name = "Test",
                LastUse = DateTime.Now.AddDays(-3),
                Words = new List<Word>() {
                    new Word() { Text = "word1", LastUse = DateTime.Now.AddDays(-3), Views = 1 },
                    new Word() { Text = "word2", LastUse = DateTime.Now.AddDays(-3), Views = 1 },
                    new Word() { Text = "word3", LastUse = DateTime.MinValue, Views = 0 },
                    new Word() { Text = "word4", LastUse = DateTime.MinValue, Views = 0 },
                    new Word() { Text = "word5", LastUse = DateTime.MinValue, Views = 0 }
                }
            };

            Category updatedBatch = service.UpdateBatch(batch);

            Assert.IsTrue(updatedBatch.LastUse.Date == DateTime.Now.Date);
            foreach (Word w in updatedBatch.Words)
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
            //List<Category> all = service.GetCategories();
            List<Category> recent = service.GetRecent();
            Assert.AreEqual(service.RecentCount, recent.Count);

            //var batches = recent.Select(c => service.BuildBatchFromCategory(c));
            //Assert.AreEqual(service.RecentCount, batches.Count());

        }

        [Test()]
        public void BuildBatchFromCategoryTest()
        {
            Category ctg = service.GetFullCategory(1);
            Assert.IsNotNull(ctg);

            Category batch1 = service.BuildBatchFromCategory(ctg);
            Assert.IsNotNull(batch1);
            Assert.AreEqual(service.BatchSize, batch1.Words.Count);

            batch1 = service.UpdateBatch(batch1);
            var views = batch1.Words.Select(w => w.Views);
            Assert.AreEqual(1, views.ElementAt(0));
            Assert.False(views.Any(o => o != views.ElementAt(0)));

            // Second pass
            Category batch2 = service.BuildBatchFromCategory(ctg);
            Assert.IsNotNull(batch2);
            Assert.AreEqual(service.BatchSize, batch2.Words.Count);

            // Both batches should have the same words.
            var words1 = batch1.Words.Select(w => w.Id);
            var words2 = batch2.Words.Select(w => w.Id);
            Assert.IsFalse(words1.Except(words2).Any());

            batch2 = service.UpdateBatch(batch2);
            views = batch2.Words.Select(w => w.Views);
            Assert.AreEqual(2, views.ElementAt(0));
            Assert.False(views.Any(o => o != views.ElementAt(0)));

            // Third pass
            Category batch3 = service.BuildBatchFromCategory(ctg);
            Assert.IsNotNull(batch3);
            Assert.AreEqual(service.BatchSize, batch3.Words.Count);

            // Both batches should have the same words.
            var words3 = batch3.Words.Select(w => w.Id);
            Assert.IsFalse(words2.Except(words3).Any());

            batch3 = service.UpdateBatch(batch3);
            views = batch3.Words.Select(w => w.Views);
            Assert.AreEqual(3, views.ElementAt(0));
            Assert.False(views.Any(o => o != views.ElementAt(0)));








        }
    }
}