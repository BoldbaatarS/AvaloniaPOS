using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Protos;
using Infrastructure.Sqlite;

public class SyncWorker : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<SyncWorker> _log;
    private readonly SyncService.SyncServiceClient _client;

    private long _lastStamp = 0; // TODO: persist to file/db

    public SyncWorker(IServiceProvider sp, ILogger<SyncWorker> log, SyncService.SyncServiceClient client)
    {
        _sp = sp; _log = log; _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("gRPC Sync started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Pull(stoppingToken);
                await Push(stoppingToken);
            }
            catch (Exception ex) { _log.LogError(ex, "Sync error"); }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    async Task Pull(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Infrastructure.Sqlite.AppDbContext>();

        using var call = _client.GetChanges(new Since { SinceMs = _lastStamp }, cancellationToken: ct);
        await foreach (var env in call.ResponseStream.ReadAllAsync(ct))
        {
            foreach (var h in env.Halls) await db.UpsertHallAsync(h);
            foreach (var t in env.Tables) await db.UpsertTableAsync(t);
            await db.SaveChangesAsync(ct);
            _lastStamp = env.ServerTimestamp;
        }
    }

    async Task Push(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Infrastructure.Sqlite.AppDbContext>();

        var sinceUtc = DateTimeOffset.FromUnixTimeMilliseconds(_lastStamp).UtcDateTime;

        var halls = await db.Halls.IgnoreQueryFilters()
            .Where(e => EF.Property<DateTime>(e,"UpdatedAt") > sinceUtc)
            .Select(e => new Hall {
                Id=e.Id.ToString(), Name=e.Name, ImagePath=e.ImagePath ?? "",
                IsDeleted = EF.Property<bool>(e,"IsDeleted"),
                UpdatedAt = new DateTimeOffset(EF.Property<DateTime>(e,"UpdatedAt"),TimeSpan.Zero).ToUnixTimeMilliseconds()
            }).ToListAsync(ct);

        var tables = await db.Tables.IgnoreQueryFilters()
            .Where(e => EF.Property<DateTime>(e,"UpdatedAt") > sinceUtc)
            .Select(e => new Table {
                Id=e.Id.ToString(), Name=e.Name, HallId=e.HallId.ToString(),
                PositionX=e.PositionX, PositionY=e.PositionY, ImagePath=e.ImagePath ?? "",
                IsDeleted = EF.Property<bool>(e,"IsDeleted"),
                UpdatedAt = new DateTimeOffset(EF.Property<DateTime>(e,"UpdatedAt"),TimeSpan.Zero).ToUnixTimeMilliseconds()
            }).ToListAsync(ct);

        if (halls.Count==0 && tables.Count==0) return;

        using var call = _client.PushChanges(cancellationToken: ct);
        await call.RequestStream.WriteAsync(new Envelope { Halls = { halls }, Tables = { tables }, ServerTimestamp=_lastStamp });
        await call.RequestStream.CompleteAsync();
        var ack = await call.ResponseAsync;
        _lastStamp = ack.ServerTimestamp;
    }
}
