using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Protos;

public sealed class SyncGrpcService : SyncService.SyncServiceBase
{
    private readonly CloudDbContext _db;
    public SyncGrpcService(CloudDbContext db) => _db = db;

    public override async Task GetChanges(Since request, IServerStreamWriter<Envelope> responseStream, ServerCallContext context)
    {
        var sinceUtc = DateTimeOffset.FromUnixTimeMilliseconds(request.SinceMs).UtcDateTime;

        var halls = await _db.Halls.WhereEFUpdatedAfter(sinceUtc).SelectSyncHall().ToListAsync();
        var tables = await _db.Tables.WhereEFUpdatedAfter(sinceUtc).SelectSyncTable().ToListAsync();
        var max = await _db.MaxUpdatedAtAsync();

        await responseStream.WriteAsync(new Envelope {
            ServerTimestamp = max, Halls = { halls }, Tables = { tables }
        });
        // Хожим нь server push (watch) хийх бол change feed-аар давталт хийнэ.
    }

    public override async Task<Ack> PushChanges(IAsyncStreamReader<Envelope> requestStream, ServerCallContext context)
    {
        await foreach (var env in requestStream.ReadAllAsync(context.CancellationToken))
        {
            // Halls upsert
            foreach (var h in env.Halls)
                await _db.UpsertHallAsync(h);

            // Tables upsert
            foreach (var t in env.Tables)
                await _db.UpsertTableAsync(t);
        }
        await _db.SaveChangesAsync();

        var max = await _db.MaxUpdatedAtAsync();
        return new Ack { ServerTimestamp = max };
    }
}
