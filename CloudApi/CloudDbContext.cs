using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Protos;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CloudApi;

public class CloudDbContext : DbContext
{
    public CloudDbContext(DbContextOptions<CloudDbContext> o) : base(o) { }

    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<HallModel> Halls => Set<HallModel>();
    public DbSet<TableModel> Tables => Set<TableModel>();
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<Branch> Branches { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Category> Categories => Set<Category>();
     
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<HallModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
        mb.Entity<HallModel>().Property<bool>("IsDeleted").HasDefaultValue(false);

        mb.Entity<TableModel>().Property<DateTime>("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
        mb.Entity<TableModel>().Property<bool>("IsDeleted").HasDefaultValue(false);

        mb.Entity<HallModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);
        mb.Entity<TableModel>().HasQueryFilter(e => EF.Property<bool>(e, "IsDeleted") == false);

        mb.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnType("decimal(18,2)");

        // Product → Category (NO CASCADE)
        mb.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category → Parent (Self reference, NO CASCADE)
        mb.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category → Branch (Cascade allowed)
        mb.Entity<Category>()
            .HasOne(c => c.Branch)
            .WithMany(b => b.Categories)
            .HasForeignKey(c => c.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Branch → Company (Cascade allowed)
        mb.Entity<Branch>()
            .HasOne(b => b.Company)
            .WithMany(c => c.Branches)
            .HasForeignKey(b => b.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);



        base.OnModelCreating(mb);
    }

    public override int SaveChanges()
    {
        Touch();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        Touch();
        return base.SaveChangesAsync(ct);
    }

    void Touch()
    {
        var now = DateTime.UtcNow;
        foreach (var e in ChangeTracker.Entries())
        {
            if (e.Metadata.FindProperty("UpdatedAt") != null &&
                (e.State is EntityState.Added or EntityState.Modified))
            {
                e.Property("UpdatedAt").CurrentValue = now;
            }
        }
    }
}

static class SyncEf
{
    public static IQueryable<T> WhereEFUpdatedAfter<T>(this IQueryable<T> q, DateTime since) where T : class
        => q.IgnoreQueryFilters().Where(e => EF.Property<DateTime>(e, "UpdatedAt") > since);

    public static async Task<long> MaxUpdatedAtAsync(this DbContext db)
    {
        var halls = await db.Set<HallModel>()
            .IgnoreQueryFilters()
            .Select(e => EF.Property<DateTime>(e, "UpdatedAt"))
            .ToListAsync();

        var tables = await db.Set<TableModel>()
            .IgnoreQueryFilters()
            .Select(e => EF.Property<DateTime>(e, "UpdatedAt"))
            .ToListAsync();

        var h = halls.DefaultIfEmpty(DateTime.MinValue).Max();
        var t = tables.DefaultIfEmpty(DateTime.MinValue).Max();

        var max = (h > t) ? h : t;
        return new DateTimeOffset(max, TimeSpan.Zero).ToUnixTimeMilliseconds();
    }

    public static IQueryable<Shared.Protos.Hall> SelectSyncHall(this IQueryable<Shared.Models.HallModel> q)
    => q.IgnoreQueryFilters().Select(e => new Shared.Protos.Hall {
        Id        = e.Id.ToString(),
        Name      = e.Name,
        ImagePath = e.ImagePath ?? "",
        IsDeleted = EF.Property<bool>(e, "IsDeleted"), // зөв
        UpdatedAt = new DateTimeOffset(
                        EF.Property<DateTime>(e, "UpdatedAt"), 
                        TimeSpan.Zero).ToUnixTimeMilliseconds()
    });
   public static IQueryable<Shared.Protos.Table> SelectSyncTable(this IQueryable<Shared.Models.TableModel> q)
    => q.IgnoreQueryFilters().Select(e => new Shared.Protos.Table {
        Id        = e.Id.ToString(),
        Name      = e.Name,
        HallId    = e.HallId.ToString(),
        PositionX = e.PositionX,
        PositionY = e.PositionY,
        ImagePath = e.ImagePath ?? "",
        IsDeleted = EF.Property<bool>(e, "IsDeleted"), // зөв
        UpdatedAt = new DateTimeOffset(
                        EF.Property<DateTime>(e, "UpdatedAt"),
                        TimeSpan.Zero).ToUnixTimeMilliseconds()
    });
    
    public static async Task UpsertHallAsync(this CloudDbContext db, Hall h)
    {
        var id = Guid.Parse(h.Id);
        var entity = await db.Halls.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);

        if (h.IsDeleted)
        {
            if (entity != null) db.Entry(entity).Property("IsDeleted").CurrentValue = true;
            return;
        }

        if (entity == null)
        {
            entity = new HallModel { Id = id, Name = h.Name ?? "", ImagePath = h.ImagePath ?? "" };
            db.Halls.Add(entity);
        }
        else
        {
            entity.Name = h.Name ?? entity.Name;
            entity.ImagePath = h.ImagePath ?? entity.ImagePath;
            db.Entry(entity).Property("IsDeleted").CurrentValue = false;
        }

        db.Entry(entity).Property("UpdatedAt").CurrentValue =
            DateTimeOffset.FromUnixTimeMilliseconds(h.UpdatedAt).UtcDateTime;
    }

    public static async Task UpsertTableAsync(this CloudDbContext db, Table t)
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
            entity = new TableModel
            {
                Id = id,
                Name = t.Name ?? "",
                HallId = Guid.Parse(t.HallId),
                PositionX = t.PositionX,
                PositionY = t.PositionY,
                ImagePath = t.ImagePath ?? "" 
            };
            db.Tables.Add(entity);
        }
        else
        {
            entity.Name = t.Name ?? entity.Name;
            entity.HallId = Guid.Parse(t.HallId);
            entity.PositionX = t.PositionX;
            entity.PositionY = t.PositionY;
            entity.ImagePath = t.ImagePath ?? entity.ImagePath ?? "";
            db.Entry(entity).Property("IsDeleted").CurrentValue = false;
        }

        db.Entry(entity).Property("UpdatedAt").CurrentValue =
            DateTimeOffset.FromUnixTimeMilliseconds(t.UpdatedAt).UtcDateTime;
    }
}
