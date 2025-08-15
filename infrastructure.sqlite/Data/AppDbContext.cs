using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Sqlite
{
    public class AppDbContext : DbContext
    {
        // 🔧 DI-д зориулсан ctor (заавал)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }



        // DbSets
        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<HallModel> Halls => Set<HallModel>();
        public DbSet<TableModel> Tables => Set<TableModel>();

        // 🔁 DI-ээр ирээгүй (design-time, тусдаа хэрэглэх үед) fallback хийхийг зөвшөөрнө
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = DbPathProvider.GetDatabasePath();    // 👈 ижил зам
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HallModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<HallModel>().Property<bool>("IsDeleted").HasDefaultValue(false);
            modelBuilder.Entity<TableModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<TableModel>().Property<bool>("IsDeleted").HasDefaultValue(false);

            modelBuilder.Entity<HallModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
            modelBuilder.Entity<TableModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);


            // Анхны өгөгдөл (HasData)
            modelBuilder.Entity<UserModel>().HasData(
                new UserModel
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Admin",
                    Pin = "0000",
                    IsAdmin = true
                },
                new UserModel
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Guest",
                    Pin = "1234",
                    IsAdmin = false
                }
            );


        }
        public override int SaveChanges()
        {
            TouchUpdatedAt();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            TouchUpdatedAt();
            return base.SaveChangesAsync(ct);
        }
        private void TouchUpdatedAt()
        {
            var now = DateTime.UtcNow;
            foreach (var e in ChangeTracker.Entries())
            {
                if (e.Metadata.FindProperty("UpdatedAt") != null &&
                   (e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    e.Property("UpdatedAt").CurrentValue = now;
                }
            }
        }
        
        
    }
}
