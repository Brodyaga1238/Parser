
using Microsoft.EntityFrameworkCore;

namespace Parser
{
    public class Db
    {
        public static async Task AddImageToDb(ProductIImage product1)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.ProductImages.Add(product1);
                await db.SaveChangesAsync();
            }
        }
        public static async Task AddDb(Product product1)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                db.ProductsDb.Add(product1);
                await db.SaveChangesAsync();
                Console.WriteLine("Объекты успешно сохранены");
            }
        }
    }
    
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            using (var db = new ApplicationContext())
            {
                if (!db.Database.CanConnect())
                {
                    Console.WriteLine("База данных не существует. Создание новой базы данных...");
                    db.Database.EnsureCreated();
                }
            }
        }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Product> ProductsDb { get; set; }
        public DbSet<ProductIImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductIImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ProductDb.db;");
        }
    }

}
    