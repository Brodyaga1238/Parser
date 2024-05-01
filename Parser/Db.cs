
using Microsoft.EntityFrameworkCore;

namespace Parser
{
    public class Db
    {
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
        public DbSet<Product> ProductsDb => Set<Product>();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ProductDb.db;");
        }
        
    }
}
    