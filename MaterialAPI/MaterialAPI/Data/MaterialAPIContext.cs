using MaterialAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace MaterialAPI.Data
{
    public class MaterialAPIContext : DbContext
    {
        public MaterialAPIContext (DbContextOptions<MaterialAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Item> Items { get; set; } = default!;
    }
}
