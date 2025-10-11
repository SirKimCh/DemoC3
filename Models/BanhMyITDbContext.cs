using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BanhMyIT.Data;

namespace BanhMyIT.Models
{
    public class BanhMyITDbContext : IdentityDbContext<ApplicationUser>
    {
        public BanhMyITDbContext(DbContextOptions<BanhMyITDbContext> options) 
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> AppUsers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Province>().HasData(
                new Province { ProvinceId = 1, Name = "Hà Nội" },
                new Province { ProvinceId = 2, Name = "TP Hồ Chí Minh" },
                new Province { ProvinceId = 3, Name = "Đà Nẵng" },
                new Province { ProvinceId = 4, Name = "Cần Thơ" }
            );
            modelBuilder.Entity<District>().HasData(
                // Hà Nội
                new District { DistrictId = 1, Name = "Quận Ba Đình", ProvinceId = 1 },
                new District { DistrictId = 2, Name = "Quận Hoàn Kiếm", ProvinceId = 1 },
                new District { DistrictId = 3, Name = "Quận Đống Đa", ProvinceId = 1 },
                // TP HCM
                new District { DistrictId = 4, Name = "Quận 1", ProvinceId = 2 },
                new District { DistrictId = 5, Name = "Quận 3", ProvinceId = 2 },
                new District { DistrictId = 6, Name = "Quận 7", ProvinceId = 2 },
                // Đà Nẵng
                new District { DistrictId = 7, Name = "Quận Hải Châu", ProvinceId = 3 },
                new District { DistrictId = 8, Name = "Quận Thanh Khê", ProvinceId = 3 },
                new District { DistrictId = 9, Name = "Quận Sơn Trà", ProvinceId = 3 },
                // Cần Thơ
                new District { DistrictId = 10, Name = "Quận Ninh Kiều", ProvinceId = 4 },
                new District { DistrictId = 11, Name = "Quận Ô Môn", ProvinceId = 4 },
                new District { DistrictId = 12, Name = "Quận Bình Thuỷ", ProvinceId = 4 },
                new District { DistrictId = 13, Name = "Quận Cái Răng", ProvinceId = 4 },
                new District { DistrictId = 14, Name = "Quận Thốt Nốt", ProvinceId = 4 },
                new District { DistrictId = 15, Name = "Huyện Vĩnh Thạnh", ProvinceId = 4 },
                new District { DistrictId = 16, Name = "Huyện Cờ Đỏ", ProvinceId = 4 },
                new District { DistrictId = 17, Name = "Huyện Phong Điền", ProvinceId = 4 },
                new District { DistrictId = 18, Name = "Huyện Thới Lai", ProvinceId = 4 }
            );
            // Indexes
            modelBuilder.Entity<Province>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<District>().HasIndex(d => new { d.ProvinceId, d.Name }).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => new { u.ProvinceId, u.DistrictId });

            // Relationship configuration to prevent multiple cascade paths
            modelBuilder.Entity<User>()
                .HasOne(u => u.Province)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasOne(u => u.District)
                .WithMany()
                .HasForeignKey(u => u.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bill & BillDetail
            modelBuilder.Entity<BillDetail>()
                .HasOne(d => d.Bill)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(d => d.BillID)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BillDetail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cart & CartDetail
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Cart)
                .WithMany(c => c.CartDetails)
                .HasForeignKey(cd => cd.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Product)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(cd => cd.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
