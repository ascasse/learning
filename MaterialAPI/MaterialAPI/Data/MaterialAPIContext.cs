using MaterialAPI.Model;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace MaterialAPI.Data
{
    public class MaterialAPIContext : DbContext
    {
        public MaterialAPIContext (DbContextOptions<MaterialAPIContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\develop\material_Raquel\learning_dotnet\MaterialAPI\MaterialAPI\material.db3");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Item> Items { get; set; } = default!;
    }
}
