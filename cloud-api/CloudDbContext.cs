using Microsoft.EntityFrameworkCore;
using Shared.Models;       // HallModel, TableModel, UserModel гэх мэт
using System;
using System.IO;


public class CloudDbContext : DbContext
{
    public CloudDbContext(DbContextOptions<CloudDbContext> o) : base(o) { }
    public DbSet<Shared.Models.HallModel> Halls => Set<Shared.Models.HallModel>();
    public DbSet<Shared.Models.TableModel> Tables => Set<Shared.Models.TableModel>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Shared.Models.HallModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
        mb.Entity<Shared.Models.HallModel>().Property<bool>("IsDeleted").HasDefaultValue(false);
        mb.Entity<Shared.Models.TableModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
        mb.Entity<Shared.Models.TableModel>().Property<bool>("IsDeleted").HasDefaultValue(false);

        mb.Entity<Shared.Models.HallModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
        mb.Entity<Shared.Models.TableModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
        base.OnModelCreating(mb);
    }

    public override int SaveChanges() { Touch(); return base.SaveChanges(); }
    public override Task<int> SaveChangesAsync(CancellationToken ct = default) { Touch(); return base.SaveChangesAsync(ct); }
    void Touch()
    {
        var now = DateTime.UtcNow;
        foreach (var e in ChangeTracker.Entries())
            if (e.Metadata.FindProperty("UpdatedAt") != null && (e.State is EntityState.Added or EntityState.Modified))
                e.Property("UpdatedAt").CurrentValue = now;
    }
}

static class SyncEf
{
    public static IQueryable<T> WhereEFUpdatedAfter<T>(this IQueryable<T> q, DateTime since) where T:class
        => q.IgnoreQueryFilters().Where(e => EF.Property<DateTime>(e,"UpdatedAt") > since);

    public static async Task<long> MaxUpdatedAtAsync(this DbContext db)
    {
        var h = await db.Set<Shared.Models.HallModel>().IgnoreQueryFilters()
            .Select(e => EF.Property<DateTime>(e,"UpdatedAt")).DefaultIfEmpty(DateTime.MinValue).MaxAsync();
        var t = await db.Set<Shared.Models.TableModel>().IgnoreQueryFilters()
            .Select(e => EF.Property<DateTime>(e,"UpdatedAt")).DefaultIfEmpty(DateTime.MinValue).MaxAsync();
        var max = (h>t) ? h : t;
        return new DateTimeOffset(max, TimeSpan.Zero).ToUnixTimeMilliseconds();
    }

    public static IQueryable<Shared.Protos.Hall> SelectSyncHall(this IQueryable<Shared.Models.HallModel> q)
        => q.IgnoreQueryFilters().Select(e => new Shared.Protos.Hall {
            Id = e.Id.ToString(), Name = e.Name, ImagePath = e.ImagePath,
            IsDeleted = EF.Property<bool>(e,"IsDeleted"),
            UpdatedAt = new DateTimeOffset(EF.Property<DateTime>(e,"UpdatedAt"), TimeSpan.Zero).ToUnixTimeMilliseconds()
        });

    public static IQueryable<Shared.Protos.Table> SelectSyncTable(this IQueryable<Shared.Models.TableModel> q)
        => q.IgnoreQueryFilters().Select(e => new Shared.Protos.Table {
            Id = e.Id.ToString(), Name = e.Name, HallId = e.HallId.ToString(),
            PositionX = e.PositionX, PositionY = e.PositionY,
            ImagePath = e.ImagePath, IsDeleted = EF.Property<bool>(e,"IsDeleted"),
            UpdatedAt = new DateTimeOffset(EF.Property<DateTime>(e,"UpdatedAt"), TimeSpan.Zero).ToUnixTimeMilliseconds()
        });

    public static async Task UpsertHallAsync(this CloudDbContext db, Shared.Protos.Hall h)
    {
        var id = Guid.Parse(h.Id);
        var entity = await db.Halls.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (h.IsDeleted)
        {
            if (entity != null) db.Entry(entity).Property("IsDeleted").CurrentValue = true;
            return;
        }
        if (entity == null) { entity = new Shared.Models.HallModel { Id=id, Name=h.Name??"", ImagePath=h.ImagePath }; db.Halls.Add(entity); }
        else { entity.Name = h.Name??entity.Name; entity.ImagePath = h.ImagePath??entity.ImagePath; db.Entry(entity).Property("IsDeleted").CurrentValue = false; }
        db.Entry(entity).Property("UpdatedAt").CurrentValue = DateTimeOffset.FromUnixTimeMilliseconds(h.UpdatedAt).UtcDateTime;
    }

    public static async Task UpsertTableAsync(this CloudDbContext db, Shared.Protos.Table t)
    {
        var id = Guid.Parse(t.Id);
        var entity = await db.Tables.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (t.IsDeleted)
        {
            if (entity != null) db.Entry(entity).Property("IsDeleted").CurrentValue = true;
            return;
        }
        if (entity == null)
        {
            entity = new Shared.Models.TableModel {
                Id=id, Name=t.Name??"", HallId=Guid.Parse(t.HallId),
                PositionX=t.PositionX, PositionY=t.PositionY, ImagePath=t.ImagePath
            };
            db.Tables.Add(entity);
        }
        else
        {
            entity.Name=t.Name??entity.Name; entity.HallId=Guid.Parse(t.HallId);
            entity.PositionX=t.PositionX; entity.PositionY=t.PositionY;
            entity.ImagePath=t.ImagePath??entity.ImagePath;
            db.Entry(entity).Property("IsDeleted").CurrentValue=false;
        }
        db.Entry(entity).Property("UpdatedAt").CurrentValue = DateTimeOffset.FromUnixTimeMilliseconds(t.UpdatedAt).UtcDateTime;
    }
}
