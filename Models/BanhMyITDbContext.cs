using Microsoft.EntityFrameworkCore;

namespace BanhMyIT.Models
{
    public class BanhMyITDbContext : DbContext
    {
        public BanhMyITDbContext(DbContextOptions<BanhMyITDbContext> options) 
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Bill> Bills { get; set; }
    }
}
