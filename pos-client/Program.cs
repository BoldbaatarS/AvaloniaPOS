using Avalonia;
using Microsoft.EntityFrameworkCore;
using System;
using static RestaurantPOS.Data.AppDbContext;

namespace RestaurantPOS;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
#if ANDROID || IOS
        // Mobile platform дээр Main ашиглагдахгүй
#else
        // CLI --seed (desktop only)
        if (args.Length > 0 && args[0].ToLower() == "--seed")
        {
            using (var db = new Data.AppDbContext())
            {
                db.Database.Migrate(); // EnsureCreated биш, Migrate
                DbSeeder.Seed(db);
                Console.WriteLine("Анхны өгөгдөл нэмэгдлээ!");
            }
            return;
        }

        using (var db = new Data.AppDbContext())
            db.Database.EnsureCreated();

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
#endif
    }

    public static AppBuilder BuildAvaloniaApp()
    {
#if IOS
        return AppBuilder.Configure<App>().UseiOS().WithInterFont().LogToTrace();
#elif ANDROID
        return AppBuilder.Configure<App>().UseAndroid().WithInterFont().LogToTrace();
#else
        return AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
#endif
    }
}
