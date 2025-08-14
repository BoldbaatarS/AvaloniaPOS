using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Infrastructure.Sqlite;            // AppDbContext
using Infrastructure.Files;             // FileImageStorage
using Shared.Abstractions;              // IImageStorage, ImageStorageOptions
using RestaurantPOS.Services;           // INavigationService, NavigationService
using RestaurantPOS.ViewModels;         // VM-үүд
using RestaurantPOS.Views;              // MainWindow, LoginWindow

namespace RestaurantPOS;

public partial class App : Application
{
    public static ServiceProvider Services { get; private set; } = default!;
    public static IAuthService AuthService => Services.GetRequiredService<IAuthService>();

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        // --- 1) Build DI container ---
        var sc = new ServiceCollection();

        // Images (portable root = exe-ийн хажууд)
        sc.AddHttpClient();
        sc.Configure<ImageStorageOptions>(o =>
        {
            o.ImagesRoot = Path.Combine(AppContext.BaseDirectory, "Images");
            o.Overwrite = false;
        });
        sc.AddSingleton<IImageStorage, FileImageStorage>();

        // DbContext (SQLite)
        var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "pos.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        sc.AddDbContext<AppDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));

        // Services
        sc.AddScoped<IAuthService, AuthService>();
        sc.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        sc.AddSingleton<MainWindowViewModel>();
        sc.AddTransient<HomeViewModel>();
        sc.AddTransient<AdminPanelViewModel>();
        sc.AddTransient<AdminHallsViewModel>();
        sc.AddTransient<AdminUsersViewModel>();
        sc.AddTransient<AdminTablesViewModel>();
        sc.AddTransient<LoginWindowViewModel>();

        Services = sc.BuildServiceProvider();

        // --- 2) DB schema + optional --seed ---
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            // --seed аргумент байвал зөвхөн seed хийгээд гарах
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dlt)
            {
                var args = dlt.Args ?? Array.Empty<string>();
                if (args.Any(a => string.Equals(a, "--seed", StringComparison.OrdinalIgnoreCase)))
                {
                    // хэрэв Seeder тусдаа бол: Infrastructure.Sqlite.Seeding.DbSeeder.Seed(db);
                    AppDbContextSeed.Seed(db); // доорх жижиг helper-г ашиглавал OK
                    dlt.Shutdown();
                    return;
                }
            }
        }

        // --- 3) UI эхлүүлэх ---
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainVm = Services.GetRequiredService<MainWindowViewModel>();
            var mainWindow = new MainWindow { DataContext = mainVm };
            desktop.MainWindow = mainWindow;
            mainWindow.Show();

            // Login
            var loginVm = Services.GetRequiredService<LoginWindowViewModel>();
            var login = new LoginWindow { DataContext = loginVm };
            var ok = await login.ShowDialog<bool>(mainWindow);
            if (!ok)
            {
                desktop.MainWindow.Close();
                return;
            }

            mainVm.CurrentUser   = AuthService.CurrentUser?.Name;
            mainVm.StatusMessage = $"Нэвтэрсэн: {mainVm.CurrentUser}";
            mainVm.IsAdmin       = AuthService.CurrentUser?.IsAdmin ?? false;
        }

        base.OnFrameworkInitializationCompleted();
    }
}

// Жижиг seed helper (хүсвэл Infrastructure.Sqlite руугаа зөөж болно)
internal static class AppDbContextSeed
{
    public static void Seed(AppDbContext db)
    {
        if (!db.Halls.Any())
        {
            db.Halls.Add(new Shared.Models.HallModel { Id = Guid.NewGuid(), Name = "Үндсэн заал" });
            db.SaveChanges();
        }
    }
}
