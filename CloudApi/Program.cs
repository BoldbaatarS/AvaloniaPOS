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
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;

    });
});

// Config унших
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CloudDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
        sqlOptions.CommandTimeout(180);
    }));

builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// DB migrate автоматаар хийх
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CloudDbContext>();
    db.Database.Migrate();
}

// Swagger зөвхөн Development дээр идэвхтэй болгож болно
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// 📌 Middleware-ээ энд оруулна
app.UseMiddleware<CloudApi.Middlewares.ExceptionMiddleware>();

app.MapGrpcService<SyncGrpcService>();
app.MapControllers();   // REST API endpoint-ууд
app.MapGet("/", () => "gRPC server running with MSSQL");



app.Run();
