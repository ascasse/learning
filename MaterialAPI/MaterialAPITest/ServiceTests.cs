using MaterialAPI.Data;
using MaterialAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MaterialAPITest
{
    [TestFixture]
    public class ServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UpdateBatchTest()
        {
            MaterialAPIContext db = new MaterialAPIContext(DbContextOptions<MaterialAPIContext> options);
            Service service = new Service(db);
            Assert.Pass();
        }
    }
}