using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure.Sqlite;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<AppDbContext>(); // таны байгаа context
        services.AddGrpcClient<Shared.Protos.SyncService.SyncServiceClient>(o =>
        {
            o.Address = new Uri(ctx.Configuration["Cloud:BaseUrl"] ?? "http://localhost:5001");
        });
        services.AddHostedService<SyncWorker>();
    })
    .Build()
    .Run();
