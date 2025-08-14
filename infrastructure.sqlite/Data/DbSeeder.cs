using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Sqlite.Seeding;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Миграци гүйцээнэ (idempotent)
        db.Database.Migrate();

        // Жишээ анхны өгөгдөл — хүсвэл өөрчил.
        if (!db.Halls.Any())
        {
            db.Halls.Add(new HallModel { Name = "Үндсэн заал" });
        }

        db.SaveChanges();
    }
}
