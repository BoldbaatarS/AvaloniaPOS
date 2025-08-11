using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Models;

namespace RestaurantPOS.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<HallModel> Halls { get; set; }
    public DbSet<TableModel> Tables { get; set; }

   

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Bin фолдерт хадгалах
        var dbPath = Path.Combine(AppContext.BaseDirectory, "appdata.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Анхны өгөгдөл (STATIC GUID ашиглана)
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

    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (!db.Halls.Any())
            {
                db.Halls.Add(new HallModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Үндсэн Заал",
                    ImagePath = null
                });
                db.SaveChanges();
            }
        }
    }
}
