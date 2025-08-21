using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls;
using RestaurantPOS.Services;
using System.Threading.Tasks;
using RestaurantPOS.Views.Dialogs;

namespace RestaurantPOS.ViewModels;

public partial class LoginWindowViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    [ObservableProperty] private string pin = string.Empty;

    public LoginWindowViewModel(IAuthService auth)
    {
        _auth = auth;
    }

    [RelayCommand]
    private void AddChar(string c) => Pin += c;

    [RelayCommand]
    private void Backspace()
    {
        if (!string.IsNullOrEmpty(Pin))
            Pin = Pin.Substring(0, Pin.Length - 1);
    }

    [RelayCommand]
    private void Clear() => Pin = string.Empty;

    [RelayCommand]
    private async Task Login(Window window)
    {
        if (_auth.LoginByPin(Pin))
        {
            window.Close(true);
        }
        else
        {
            await MessageBoxService.ShowAsync(window, "Алдаа", "Буруу PIN!", MessageDialog.DialogType.Error);
            Pin = string.Empty;
        }
    }
}
