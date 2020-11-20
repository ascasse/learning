using NUnit.Framework;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;
using Learning.Model;

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
    }
}