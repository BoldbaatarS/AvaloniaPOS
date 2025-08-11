using System.Threading.Tasks;
using Avalonia.Controls;

namespace RestaurantPOS.Services;

public static class UserSessionManager
{
    public static string? Username { get; private set; }
    public static bool IsAdmin { get; private set; }

    public static async Task<bool> RequireLoginAsync(Window? owner = null)
    {
        var loginWindow = new RestaurantPOS.Views.LoginWindow
        {
            DataContext = new RestaurantPOS.ViewModels.LoginWindowViewModel(App.AuthService)
        };
        var result = await loginWindow.ShowDialog<bool>(owner!);
        if (result && App.AuthService.CurrentUser != null)
        {
            Username = App.AuthService.CurrentUser.Name;
            IsAdmin = App.AuthService.CurrentUser.IsAdmin;
        }
        return result;
    }
}

