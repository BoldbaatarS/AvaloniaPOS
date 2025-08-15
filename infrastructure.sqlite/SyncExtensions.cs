using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Models;            // HallModel, TableModel
using Shared.Protos;


namespace Infrastructure.Sqlite;

public static class SyncExtensions
{
    public static async Task UpsertHallAsync(this AppDbContext db, Shared.Protos.Hall h, CancellationToken ct = default)
    {
        var id = Guid.Parse(h.Id);
        var entity = await db.Halls.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        if (h.IsDeleted)
        {
            if (entity != null)
                db.Entry(entity).Property("IsDeleted").CurrentValue = true;
        }
        else
        {
            if (entity == null)
            {
                entity = new HallModel { Id = id, Name = h.Name ?? "", ImagePath = h.ImagePath };
                db.Halls.Add(entity);
            }
            else
            {
                entity.Name      = h.Name      ?? entity.Name;
                entity.ImagePath = h.ImagePath ?? entity.ImagePath;
                db.Entry(entity).Property("IsDeleted").CurrentValue = false;
            }
        }

        var ts = DateTimeOffset.FromUnixTimeMilliseconds(h.UpdatedAt).UtcDateTime;
        if (entity != null)
            db.Entry(entity).Property("UpdatedAt").CurrentValue = ts;
    }

    public static async Task UpsertTableAsync(this AppDbContext db, Shared.Protos.Table t, CancellationToken ct = default)
    {
        var id = Guid.Parse(t.Id);
        var entity = await db.Tables.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        if (t.IsDeleted)
        {
            if (entity != null)
                db.Entry(entity).Property("IsDeleted").CurrentValue = true;
        }
        else
        {
            if (entity == null)
            {
                entity = new TableModel
                {
                    Id        = id,
                    Name      = t.Name ?? "",
                    HallId    = Guid.Parse(t.HallId),
                    PositionX = t.PositionX,
                    PositionY = t.PositionY,
                    ImagePath = t.ImagePath
                };
                db.Tables.Add(entity);
            }
            else
            {
                entity.Name      = t.Name ?? entity.Name;
                entity.HallId    = Guid.Parse(t.HallId);
                entity.PositionX = t.PositionX;
                entity.PositionY = t.PositionY;
                entity.ImagePath = t.ImagePath ?? entity.ImagePath;
                db.Entry(entity).Property("IsDeleted").CurrentValue = false;
            }
        }

        var ts = DateTimeOffset.FromUnixTimeMilliseconds(t.UpdatedAt).UtcDateTime;
        if (entity != null)
            db.Entry(entity).Property("UpdatedAt").CurrentValue = ts;
    }
}
