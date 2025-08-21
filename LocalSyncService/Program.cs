using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Sqlite;
using Shared.Utils;


var cfg = ConfigManager.Load();


Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // === POS-той ижил DB руу заана ===
       
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={cfg.DatabasePath}"));
       
        // Cloud API клиент
        services.AddGrpcClient<Shared.Protos.SyncService.SyncServiceClient>(o =>
        {
            o.Address = new Uri(ctx.Configuration["Cloud:BaseUrl"] ?? "http://localhost:5001");
            
        });

        // Worker
        services.AddHostedService<SyncWorker>();
    })
    .Build()
    .Run();
