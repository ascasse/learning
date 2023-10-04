using MaterialAPI.Data;
using MaterialAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace MaterialAPITest
{
    [TestFixture]
    public class ServiceTests
    {
        DbContextOptions<MaterialAPIContext> options;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UpdateBatchTest()
        {
            MaterialAPIContext db = new MaterialAPIContext(options);
            Service service = new Service(db);
            Assert.Pass();
        }
    }
}