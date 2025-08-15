using Microsoft.EntityFrameworkCore;
using Shared.Protos;

var builder = WebApplication.CreateBuilder(args);

// HTTP/2
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(5001, lo => lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2));

builder.Services.AddGrpc();
builder.Services.AddDbContext<CloudDbContext>(o =>
    o.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "cloud.db")}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<CloudDbContext>().Database.Migrate();

app.MapGrpcService<SyncGrpcService>();
app.MapGet("/", () => "gRPC server.");
app.Run();
