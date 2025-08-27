using Shared.Protos;
using CloudApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Kestrel дээр HTTP/2 асааж 5001 дээр сонсох
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Config унших
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddGrpc();
builder.Services.AddDbContext<CloudDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Program));
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
