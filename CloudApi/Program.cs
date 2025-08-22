using Shared.Protos;
using CloudApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddGrpc();
builder.Services.AddDbContext<CloudDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
