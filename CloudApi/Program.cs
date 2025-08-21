using Shared.Protos;
using CloudApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// HTTP/2
builder.WebHost.ConfigureKestrel(o =>
    o.ListenAnyIP(5001, lo => lo.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2));

// Add services
builder.Services.AddGrpc();
builder.Services.AddDbContext<CloudDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("CloudDb")));

var app = builder.Build();

// DB migrate автоматаар хийх
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CloudDbContext>();
    db.Database.Migrate();
}

app.MapGrpcService<SyncGrpcService>();
app.MapGet("/", () => "gRPC server running with MSSQL");

app.Run();
