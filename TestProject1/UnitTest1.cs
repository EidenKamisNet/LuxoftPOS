using LuxoftPOS;
using Microsoft.EntityFrameworkCore;

namespace TestProject1
{
    public class UnitTest1
    {

        [Fact]
        public void Test1()
        {
            var dbName = $"AuthorPostsDb_{DateTime.Now.ToFileTimeUtc()}";
            dbContextOptions = new DbContextOptionsBuilder<EFCashMastersDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }
    }
}