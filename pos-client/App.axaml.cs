using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RestaurantPOS.Data;
using RestaurantPOS.Services;
using RestaurantPOS.ViewModels;
using RestaurantPOS.Views;

namespace RestaurantPOS;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public static IAuthService AuthService { get; private set; }=null!;

    public override async void OnFrameworkInitializationCompleted()
    {
        AuthService = new AuthService(new AppDbContext());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 1. MainWindow-г эхлээд оноо
            var navigationService = new NavigationService();
            var mainVm = new MainWindowViewModel(navigationService);
            var mainWindow = new MainWindow { DataContext = mainVm };
            desktop.MainWindow = mainWindow;

            mainWindow.Show(); // MainWindow-г харагдуулна

            // 2. Login-г MainWindow-г owner болгож дуудах
            var login = new RestaurantPOS.Views.LoginWindow
            {
                DataContext = new LoginWindowViewModel(AuthService)
            };
            var success = await login.ShowDialog<bool>(mainWindow);
            if (!success)
            {
                desktop.MainWindow.Close(); // Хэрэв login амжилтгүй бол програмаас гарах
                return;
            }



            // 3. Нэвтэрсэн хэрэглэгчийн мэдээллийг оноох
            mainVm.CurrentUser = AuthService.CurrentUser?.Name;
            mainVm.StatusMessage = $"Нэвтэрсэн: {mainVm.CurrentUser}";
            mainVm.IsAdmin = AuthService.CurrentUser?.IsAdmin ?? false;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
