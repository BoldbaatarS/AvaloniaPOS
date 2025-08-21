using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Protos;

namespace CloudApi;

public class SyncGrpcService : SyncService.SyncServiceBase
{
    private readonly CloudDbContext _db;

    public SyncGrpcService(CloudDbContext db)
    {
        _db = db;
    }

    public override async Task GetChanges(Since request, IServerStreamWriter<Envelope> responseStream, ServerCallContext context)
    {
        var since = DateTimeOffset.FromUnixTimeMilliseconds(request.SinceMs).UtcDateTime;

        var halls = await _db.Halls
            .WhereEFUpdatedAfter(since)
            .SelectSyncHall().ToListAsync(context.CancellationToken);

        var tables = await _db.Tables
            .WhereEFUpdatedAfter(since)
            .SelectSyncTable().ToListAsync(context.CancellationToken);

        var env = new Envelope
        {
            ServerTimestamp = await _db.MaxUpdatedAtAsync()
        };
        env.Halls.AddRange(halls);
        env.Tables.AddRange(tables);

        await responseStream.WriteAsync(env);
    }

    public override async Task<Ack> PushChanges(IAsyncStreamReader<Envelope> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext(context.CancellationToken))
        {
            var env = requestStream.Current;

            foreach (var h in env.Halls)
                await _db.UpsertHallAsync(h);

            foreach (var t in env.Tables)
                await _db.UpsertTableAsync(t);

            await _db.SaveChangesAsync(context.CancellationToken);
        }

        return new Ack { ServerTimestamp = await _db.MaxUpdatedAtAsync() };
    }
}
